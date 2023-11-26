using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using static System.Formats.Asn1.AsnWriter;

namespace ThreeThingGame
{
    internal class Game_Screen
    {
        // Constants
        private const uint MAX_SPEED = 3;
        private const float DECELERATION_RATE = 0.5f;
        private const float GRAVITY = 0.2f;
        private const uint TERMINAL_VELOCITY = 3;
        private const float COAL_CAPACITY = 5;

        // External Variables
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Internal Variables
        // General
        private double timeLeft;
        private Ground ground;
        private Ground unplayableGround;
        private Ground lowestGround;
        private Vector2 cameraPosition;
        private Vector2 snowPosition;
        private float snowSpeed;

        // Players
        private Player player1;
        private Player player2;

        // Time left
        private string timeLeftPhrase;
        private Vector2 timeLeftPos;
        
        // Constructor
        public Game_Screen(
            ref GraphicsDeviceManager graphics,
            ref SpriteBatch spriteBatch,
            int dayTime,
            Dictionary<string, SpriteFont> fonts,
            Dictionary<string, Texture2D> textures,
            uint groundWidth,
            uint groundDepth,
            float coalDist,
            float oilDist,
            float gasDist,
            float clumpSum
            )
        {
            // Assign values
            _graphics = graphics;
            _spriteBatch = spriteBatch;
            timeLeft = dayTime;

            // Create new players
            player1 = new Player(
                0,
                COAL_CAPACITY,
                new Vector2(50, 180),
                textures["Blue_Front"],
                new Vector2(60, 120)
                );
            player2 = new Player(
                1,
                COAL_CAPACITY,
                new Vector2(120, 180),
                textures["Red_Front"],
                new Vector2(60, 120)
                );

            // Create grounds
            unplayableGround = new Ground(
                Ground.GenerateGround(
                    5,
                    25,
                    0f,
                    0f,
                    0f,
                    1f,
                    0f,
                    true
                    ),
                5,
                25
                );

            lowestGround = new Ground(
                Ground.GenerateGround(
                    24,
                    14,
                    0f,
                    0f,
                    0f,
                    1f,
                    0f,
                    true
                    ),
                24,
                14
                );

            ground = new Ground(
                Ground.GenerateGround(
                    groundWidth,
                    groundDepth,
                    coalDist,
                    0f, //oilDist, disable oil and gas for now
                    0f, //gasDist,
                    0f,
                    clumpSum
                    ),
                groundWidth,
                groundDepth
                );

            // Initialise values
            cameraPosition = Vector2.Zero;
            snowPosition = new Vector2(-960, -540);
            snowSpeed = 1f;

            // Create timeLeft phrase
            timeLeftPhrase = "Until Nightfall:";
            Vector2 phraseSize = fonts["SWTxt_24"].MeasureString(timeLeftPhrase);
            timeLeftPos = new Vector2(
                940 - phraseSize.X,
                20
                );
        }

        // Methods
        public void RunLogic(
            ref State.GameState state,
            Dictionary<Keys, bool> keyMap,
            float gameSpeed,
            double deltaTime,
            Keys[] KeysPressed
            )
        {
            // Handle game timer
            timeLeft -= deltaTime;

            if (timeLeft < 0)
            {
                // Day has ended
                state = State.GameState.DayEnd_Load;
            }

            // Cache player velocities
            Vector2 tempVelPlayer1 = player1.Velocity;
            Vector2 tempVelPlayer2 = player2.Velocity;

            // Read keyboard input
            foreach (Keys key in KeysPressed)
            {
                switch (key)
                {
                    case Keys.A:
                        tempVelPlayer1.X = -(gameSpeed * MAX_SPEED);
                        break;
                    case Keys.D:
                        tempVelPlayer1.X = gameSpeed * MAX_SPEED;
                        break;
                    case Keys.S:
                        // P1 dig down

                        // If key not held
                        if (!keyMap[Keys.S])
                        {
                            (int y, int x) tile = Ground.GetNearestTileToPoint(
                                new Vector2(
                                    player1.Position.X + player1.Size.X / 2,
                                    player1.Position.Y + player1.Size.Y
                                    ),
                                ground,
                                new Rectangle(
                                    200,
                                    300,
                                    760,
                                    1000
                                    ),
                                new Vector2(
                                    player1.Size.X,
                                    player1.Size.X / 2
                                    )
                                );

                            if (tile.y >= 0 && tile.x >= 0)
                            {
                                ground.Tiles[tile.y, tile.x].Filled = false;
                            }
                        }
                        break;
                    case Keys.Left:
                        tempVelPlayer2.X = -(gameSpeed * MAX_SPEED);
                        break;
                    case Keys.Right:
                        tempVelPlayer2.X = gameSpeed * MAX_SPEED;
                        break;
                    case Keys.Down:
                        // P2 dig down
                        break;
                }
            }

            // Apply deceleration
            if (Math.Abs(tempVelPlayer1.X) > gameSpeed * DECELERATION_RATE)
            {
                tempVelPlayer1.X -= gameSpeed * DECELERATION_RATE * Math.Sign(tempVelPlayer1.X);
            }
            else if (tempVelPlayer1.X != 0)
            {
                tempVelPlayer1.X = 0;
            }
            if (Math.Abs(tempVelPlayer2.X) > gameSpeed * DECELERATION_RATE)
            {
                tempVelPlayer2.X -= gameSpeed * DECELERATION_RATE * Math.Sign(tempVelPlayer2.X);
            }
            else if (tempVelPlayer2.X != 0)
            {
                tempVelPlayer2.X = 0;
            }

            // Apply gravity
            tempVelPlayer1.Y += GRAVITY * gameSpeed;
            tempVelPlayer2.Y += GRAVITY * gameSpeed;

            // Cap by terminal velocity
            if (tempVelPlayer1.X > TERMINAL_VELOCITY)
            {
                tempVelPlayer1.X += TERMINAL_VELOCITY;
            }
            if (tempVelPlayer2.X > TERMINAL_VELOCITY)
            {
                tempVelPlayer2.X = TERMINAL_VELOCITY;
            }

            // Fetch surface tiles
            GroundTile[] mineTiles = Ground.GetSurface(
                ground,
                new Rectangle(
                    200,
                    300,
                    760,
                    1000
                    )
                );

            GroundTile[] lowTiles = Ground.GetSurface(
                ground,
                new Rectangle(
                    0,
                    1300,
                    960,
                    560
                    )
                );

            GroundTile[] baseTiles = Ground.GetSurface(
                unplayableGround,
                new Rectangle(
                    0,
                    300,
                    200,
                    1000
                    )
                );

            // Create array to contain all prior arrays
            GroundTile[] surfaceTiles = new GroundTile[mineTiles.Length + baseTiles.Length + lowTiles.Length];

            // Combine arrays
            Array.Copy(mineTiles, surfaceTiles, mineTiles.Length);
            Array.ConstrainedCopy(baseTiles, 0, surfaceTiles, mineTiles.Length, baseTiles.Length);
            Array.ConstrainedCopy(lowTiles, 0, surfaceTiles, mineTiles.Length + baseTiles.Length, lowTiles.Length);

            // Track whether move is valid
            bool[] valid = new bool[4]
            {
                true, // player1 X
                true, // player1 Y
                true, // player2 X
                true, // player2 Y
            };

            // Loop through all surface tiles
            foreach (GroundTile tile in surfaceTiles)
            {
                // PLAYER 1
                // Check if x is valid
                if (valid[0])
                {
                    Vector2 checkPos = player1.Position;
                    checkPos.X += tempVelPlayer1.X;
                    Rectangle checkRect = new Rectangle(
                        (int)checkPos.X,
                        (int)checkPos.Y,
                        (int)player1.Size.X,
                        (int)player1.Size.Y
                        );
                    if (checkRect.Intersects(tile.Rect))
                    {
                        valid[0] = false;
                    }
                }

                // Check if y is valid
                if (valid[1])
                {
                    Vector2 checkPos = player1.Position;
                    checkPos.Y += tempVelPlayer1.Y;
                    Rectangle checkRect = new Rectangle(
                        (int)checkPos.X,
                        (int)checkPos.Y,
                        (int)player1.Size.X,
                        (int)player1.Size.Y
                        );
                    if (checkRect.Intersects(tile.Rect))
                    {
                        valid[1] = false;
                    }
                }

                // PLAYER 2
                // Check if x is valid
                if (valid[2])
                {
                    Vector2 checkPos = player2.Position;
                    checkPos.X += tempVelPlayer2.X;
                    Rectangle checkRect = new Rectangle(
                        (int)checkPos.X,
                        (int)checkPos.Y,
                        (int)player2.Size.X,
                        (int)player2.Size.Y
                        );
                    if (checkRect.Intersects(tile.Rect))
                    {
                        valid[2] = false;
                    }
                }

                // Check if y is valid
                if (valid[3])
                {
                    Vector2 checkPos = player2.Position;
                    checkPos.Y += tempVelPlayer2.Y;
                    Rectangle checkRect = new Rectangle(
                        (int)checkPos.X,
                        (int)checkPos.Y,
                        (int)player2.Size.X,
                        (int)player2.Size.Y
                        );
                    if (checkRect.Intersects(tile.Rect))
                    {
                        valid[3] = false;
                    }
                }
            }

            // Cache player positions
            Vector2 tempPosPlayer1 = player1.Position;
            Vector2 tempPosPlayer2 = player2.Position;

            // Keep players within screen bounds
            if (valid[0])
            {
                valid[0] = LimitToBound(player1, tempPosPlayer1, tempVelPlayer1);
            }
            if (valid[2])
            {
                valid[2] = LimitToBound(player2, tempPosPlayer2, tempVelPlayer2);
            }

            // Apply changes to cached positions
            if (valid[0])
            {
                tempPosPlayer1.X += tempVelPlayer1.X;
            }
            else
            {
                tempVelPlayer1.X = 0;
            }
            if (valid[1])
            {
                tempPosPlayer1.Y += tempVelPlayer1.Y;
            }
            else
            {
                tempVelPlayer1.Y = 0;
            }
            if (valid[2])
            {
                tempPosPlayer2.X += tempVelPlayer2.X;
            }
            else
            {
                tempVelPlayer2.X = 0;
            }
            if (valid[3])
            {
                tempPosPlayer2.Y += tempVelPlayer2.Y;
            }
            else
            {
                tempVelPlayer2.Y = 0;
            }

            // Set player velocities
            player1.Velocity = tempVelPlayer1;
            player2.Velocity = tempVelPlayer2;

            // Apply cached positions
            player1.Position = tempPosPlayer1;
            player2.Position = tempPosPlayer2;

            // Calculate new camera Y position
            cameraPosition.Y = -200 + (player1.Position.Y + player2.Position.Y) / 2;

            // Calculate new snow position
            snowPosition.X += gameSpeed * snowSpeed;
            snowPosition.Y += gameSpeed * snowSpeed;

            if (snowPosition.X >= 0)
            {
                snowPosition.X = -960;
            }
            if (snowPosition.Y >= 0)
            {
                snowPosition.Y = -540;
            }
        }
        public void RunGraphics(
            SpriteBatch spriteBatch,
            Vector2 scale,
            Dictionary<string, Texture2D> textures,
            Dictionary<string, SpriteFont> fonts
            )
        {
            // Calculate float scale
            float minScale = Math.Min(scale.X, scale.Y);

            // Draw empties
            Ground.DrawEmpties(
                spriteBatch,
                ground.Tiles,
                ground.Width,
                ground.Depth,
                new Rectangle(
                    (int)(200 * scale.X),
                    (int)((300 - cameraPosition.Y) * scale.Y),
                    (int)(760 * scale.X),
                    (int)(1000 * scale.Y)
                    ),
                textures
                );

            // Draw player base
            spriteBatch.Draw(
                textures["GameplayBase"],
                new Rectangle(
                    0,
                    (int)((100 - cameraPosition.Y) * scale.Y),
                    (int)(200 * scale.X),
                    (int)(200 * scale.Y)
                    ),
                Color.White
                );

            // Draw players
            spriteBatch.Draw(
                player2.Texture,
                new Rectangle(
                    (int)(player2.Position.X * scale.X),
                    (int)((player2.Position.Y - cameraPosition.Y) * scale.Y),
                    (int)(player2.Size.X * scale.X),
                    (int)(player2.Size.Y * scale.Y)
                    ),
                Color.White
                );

            spriteBatch.Draw(
                player1.Texture,
                new Rectangle(
                    (int)(player1.Position.X * scale.X),
                    (int)((player1.Position.Y - cameraPosition.Y) * scale.Y),
                    (int)(player1.Size.X * scale.X),
                    (int)(player1.Size.Y * scale.Y)
                    ),
                Color.White
                );

            // Draw snow background
            spriteBatch.Draw(
                textures["Snow"],
                new Rectangle(
                    (int)(snowPosition.X * scale.X),
                    (int)(snowPosition.Y * scale.Y),
                    (int)(textures["Snow"].Width * scale.X),
                    (int)(textures["Snow"].Height * scale.Y)
                    ),
                Color.White
                );

            // Draw unplayable ground
            Ground.DrawGround(
                spriteBatch,
                unplayableGround.Tiles,
                unplayableGround.Width,
                unplayableGround.Depth,
                new Rectangle(
                    0,
                    (int)((300 - cameraPosition.Y) * scale.Y),
                    (int)(200 * scale.X),
                    (int)(1000 * scale.Y)
                    ),
                textures
                );

            // Draw lowest ground
            Ground.DrawGround(
                spriteBatch,
                lowestGround.Tiles,
                lowestGround.Width,
                lowestGround.Depth,
                new Rectangle(
                    (int)(0 * scale.X),
                    (int)((1300 - cameraPosition.Y) * scale.Y),
                    (int)(960 * scale.X),
                    (int)(560 * scale.Y)
                    ),
                textures
                );

            // Draw game ground
            Ground.DrawGround(
                spriteBatch,
                ground.Tiles,
                ground.Width,
                ground.Depth,
                new Rectangle(
                    (int)(200 * scale.X),
                    (int)((300 - cameraPosition.Y) * scale.Y),
                    (int)(760 * scale.X),
                    (int)(1000 * scale.Y)
                    ),
                textures
                );

            // UI
            // Player Info
            // Player 1
            Rectangle p1Base = new Rectangle(
                (int)(20 * scale.X),
                (int)(20 * scale.Y),
                (int)(100 * scale.X),
                (int)(100 * scale.Y)
                );
            DrawPlayerUI(
                ref spriteBatch,
                textures,
                fonts,
                player1,
                p1Base,
                scale,
                minScale
                );

            // Player 2
            Rectangle p2Base = new Rectangle(
                (int)(340 * scale.X),
                (int)(20 * scale.Y),
                (int)(100 * scale.X),
                (int)(100 * scale.Y)
                );
            DrawPlayerUI(
                ref spriteBatch,
                textures,
                fonts,
                player2,
                p2Base,
                scale,
                minScale
                );

            // Draw timer
            spriteBatch.DrawString(
                fonts["SWTxt_24"],
                timeLeftPhrase,
                timeLeftPos * scale,
                Color.White,
                0,
                Vector2.Zero,
                minScale,
                0,
                0
                );

            string timer = String.Empty;
            if (timeLeft >= 10)
            {
                timer = $"{(int)timeLeft / 60}:{(int)(timeLeft % 60)}";
            }
            else
            {
                timer = $"{(int)timeLeft / 60}:0{(int)(timeLeft % 60)}";
            }
            Vector2 timerSize = fonts["SWTxt_24"].MeasureString(timer);
            Vector2 timerPos = new Vector2(
                940 - timerSize.X,
                timeLeftPos.Y + 40
                );

            spriteBatch.DrawString(
                fonts["SWTxt_24"],
                timer,
                timerPos * scale,
                Color.White,
                0,
                Vector2.Zero,
                minScale,
                0,
                0
                );
        }
        private static bool LimitToBound(
            Player player,
            Vector2 position,
            Vector2 velocity,
            uint leftBound = 0,
            uint rightBound = 960
            )
        {
            // Check if player is about to leave left boundary
            if (position.X + velocity.X < leftBound)
            {
                return false;
            }
            else if (position.X + velocity.X > rightBound - player.Size.X)
            {
                return false;
            }

            return true;
        }
        private static void DrawPlayerUI(
            ref SpriteBatch spriteBatch,
            Dictionary<string, Texture2D> textures,
            Dictionary<string, SpriteFont> fonts,
            Player player,
            Rectangle baseRect,
            Vector2 scale,
            float minScale
            )
        {
            string iconName = String.Empty;
            switch (player.ID)
            {
                case 0: // Blue player
                    iconName = "Blue_Icon";
                    break;
                case 1: // Red Player
                    iconName = "Red_Icon";
                    break;
                default: // Default icon
                    iconName = "Blue_Icon";
                    break;
            }
            spriteBatch.Draw(
                textures[iconName],
                baseRect,
                Color.White
                );
            spriteBatch.DrawString(
                fonts["SWTxt_12"],
                $"Health: {player.Health}",
                new Vector2(
                    (baseRect.X + baseRect.Width) + 10 * scale.X,
                    (baseRect.Y) + 10 * scale.Y
                    ),
                Color.White,
                0,
                Vector2.Zero,
                minScale,
                0,
                0
                );
            spriteBatch.DrawString(
                fonts["SWTxt_12"],
                $"Coal: {Math.Round(player.HeldCoal, 1)}/{player.CoalCapacity}",
                new Vector2(
                    (baseRect.X + baseRect.Width) + 10 * scale.X,
                    (baseRect.Y) + 40 * scale.Y
                    ),
                Color.White,
                0,
                Vector2.Zero,
                minScale,
                0,
                0
                );
            spriteBatch.DrawString(
                fonts["SWTxt_12"],
                $"Temperature: {player.Temperature}C",
                new Vector2(
                    (baseRect.X + baseRect.Width) + 10 * scale.X,
                    (baseRect.Y) + 70 * scale.Y
                    ),
                Color.White,
                0,
                Vector2.Zero,
                minScale,
                0,
                0
                );
        }
    }
}
