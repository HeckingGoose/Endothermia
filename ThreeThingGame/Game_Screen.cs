﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using static System.Formats.Asn1.AsnWriter;
using Microsoft.Xna.Framework.Audio;
using System.Security.Cryptography;

namespace ThreeThingGame
{
    internal class Game_Screen
    {
        // Constants
        private const uint MAX_SPEED = 3;
        private const float DECELERATION_RATE = 0.5f;
        private const float GRAVITY = 0.2f;
        private const uint TERMINAL_VELOCITY = 3;
        private const uint COAL_CAPACITY = 5;
        private const float TARGET_TEMP = 38f;
        private const float FREEZE_RATE = 0.003f;
        private const float FATAL_TEMP = 33f;
        private const double SPRITE_RESET_TIME_1 = 0.1f;
        private const double SPRITE_RESET_TIME_2 = 0.1f;

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
        private Random rng;
        private double resetTime1 = SPRITE_RESET_TIME_1;
        private double resetTime2 = SPRITE_RESET_TIME_2;

        // Snow step timers
        private double stepTimeLeft_1 = 0;
        private double stepTimeLeft_2 = 0;

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
                "Blue",
                COAL_CAPACITY,
                new Vector2(50, 180),
                textures["Blue_Front"],
                new Vector2(60, 120)
                );
            player2 = new Player(
                "Red",
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
            rng = new Random();

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
            Dictionary<string, SoundEffect> soundEffects,
            Dictionary<string, Texture2D> textures,
            ref int totalCoal,
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

            // Decay player temperature
            player1.Temperature -= FREEZE_RATE * gameSpeed;
            player2.Temperature -= FREEZE_RATE * gameSpeed;

            // Check if player is in correct area to warm up
            if (player1.Position.X < 200)
            {
                // Handle temperature
                if (player1.Temperature < TARGET_TEMP)
                {
                    player1.Temperature += 0.3f;
                }

            }
            if (player2.Position.X < 200)
            {
                // Handle temperature
                if (player2.Temperature < TARGET_TEMP)
                {
                    player2.Temperature += 0.3f;
                }
            }

            if (player1.Temperature < FATAL_TEMP)
            {
                player1.Health -= (float)deltaTime * 5f;
            }
            if (player2.Temperature < FATAL_TEMP)
            {
                player2.Health -= (float)deltaTime * 5f;
            }

            if (player1.Health < 0 || player2.Health < 0)
            {
                state = State.GameState.Death_Load;
            }

            // If player is walking on the surface (uses hardcoded values, ew)
            if (player1.Position.Y + player1.Size.Y < 310 && Math.Abs(player1.Velocity.Length()) > 0)
            {
                // Player 1 is probably walking on the surface

                // Decrement timer
                stepTimeLeft_1 -= deltaTime;

                if (stepTimeLeft_1 <= 0)
                {
                    // If the sound effect has stopped playing, then play a new one

                    // Generate random index
                    int i = rng.Next(9);

                    // Cache length of sound at index
                    stepTimeLeft_1 = soundEffects[$"Snow_Walk_{i}"].Duration.TotalSeconds;

                    // Play sound at index
                    soundEffects[$"Snow_Walk_{i}"].Play();
                }
            }
            if (player2.Position.Y + player2.Size.Y < 310 && Math.Abs(player2.Velocity.Length()) > 0)
            {
                // Player 2 is probably walking on the surface

                // Decrement timer
                stepTimeLeft_2 -= deltaTime;

                if (stepTimeLeft_2 <= 0)
                {
                    // If the sound effect has stopped playing, then play a new one

                    // Generate random index
                    int i = rng.Next(9);

                    // Cache length of sound at index
                    stepTimeLeft_2 = soundEffects[$"Snow_Walk_{i}"].Duration.TotalSeconds;

                    // Play sound at index
                    soundEffects[$"Snow_Walk_{i}"].Play();
                }
            }
            // Set player to default state
            if (resetTime1 <= 0)
            {
                player1.SetPlayerState(
                    Player.State.Idle,
                    textures
                    );
            }
            else
            {
                resetTime1 -= deltaTime;
            }
            
            if (resetTime2 <= 0)
            {
                player2.SetPlayerState(
                Player.State.Idle,
                textures
                );
            }
            else
            {
                resetTime2 -= deltaTime;
            }

            // Read keyboard input
            foreach (Keys key in KeysPressed)
            {
                switch (key)
                {
                    case Keys.W:
                        // P1 go up

                        if (player1.Position.Y + player1.Size.Y > 300)
                        {
                            tempVelPlayer1.Y = -(gameSpeed * MAX_SPEED);
                            player1.SetPlayerState(
                                Player.State.Climbing,
                                textures
                                );
                            resetTime1 = SPRITE_RESET_TIME_1;
                        }

                        if (!keyMap[Keys.W] && player1.Velocity.Y == 0)
                        {
                            uint heldCoal = player1.HeldCoal;
                            bool success = MineTile(
                                ref heldCoal,
                                soundEffects,
                                player1,
                                ground,
                                new Vector2(
                                    player1.Position.X + player1.Size.X / 2,
                                    player1.Position.Y
                                    ),
                                new Vector2(
                                    player1.Size.X / 2,
                                    player1.Size.X / 2
                                    ),
                                ref rng
                                );
                            player1.HeldCoal = heldCoal;

                            if (success)
                            {
                                player1.SetPlayerState(
                                    Player.State.Mining_Up,
                                    textures
                                    );
                                resetTime1 = SPRITE_RESET_TIME_1;
                            }
                        }
                        break;
                    case Keys.A:
                        tempVelPlayer1.X = -(gameSpeed * MAX_SPEED);

                        if (!keyMap[Keys.A] && player1.Velocity.Y == 0)
                        {
                            uint heldCoal = player1.HeldCoal;
                            bool success = MineTile(
                                ref heldCoal,
                                soundEffects,
                                player1,
                                ground,
                                new Vector2(
                                    player1.Position.X,
                                    player1.Position.Y + player1.Size.Y / 2
                                    ),
                                new Vector2(
                                    player1.Size.X / 2,
                                    player1.Size.Y / 2
                                    ),
                                ref rng
                                );
                            player1.HeldCoal = heldCoal;

                            if (success)
                            {
                                player1.SetPlayerState(
                                    Player.State.Mining_Left,
                                    textures
                                    );
                                resetTime1 = SPRITE_RESET_TIME_1;
                            }
                        }
                        break;
                    case Keys.D:
                        tempVelPlayer1.X = gameSpeed * MAX_SPEED;

                        if (!keyMap[Keys.D] && player1.Velocity.Y == 0)
                        {
                            uint heldCoal = player1.HeldCoal;
                            bool success = MineTile(
                                ref heldCoal,
                                soundEffects,
                                player1,
                                ground,
                                new Vector2(
                                    player1.Position.X + player1.Size.X,
                                    player1.Position.Y + player1.Size.Y / 2
                                    ),
                                new Vector2(
                                    player1.Size.X / 2,
                                    player1.Size.Y / 2
                                    ),
                                ref rng
                                );
                            player1.HeldCoal = heldCoal;

                            if (success)
                            {
                                player1.SetPlayerState(
                                    Player.State.Mining_Right,
                                    textures
                                    );
                                resetTime1 = SPRITE_RESET_TIME_1;
                            }
                        }
                        break;
                    case Keys.S:
                        // P1 dig down

                        // If key not held
                        if (!keyMap[Keys.S] && player1.Velocity.Y == 0)
                        {
                            uint heldCoal = player1.HeldCoal;
                            bool success = MineTile(
                                ref heldCoal,
                                soundEffects,
                                player1,
                                ground,
                                new Vector2(
                                    player1.Position.X + player1.Size.X / 2,
                                    player1.Position.Y + player1.Size.Y
                                    ),
                                new Vector2(
                                    player1.Size.X / 2,
                                    player1.Size.X / 2
                                    ),
                                ref rng
                                );
                            player1.HeldCoal = heldCoal;

                            if (success)
                            {
                                player1.SetPlayerState(
                                    Player.State.Mining_Down,
                                    textures
                                    );
                                resetTime1 = SPRITE_RESET_TIME_1;
                            }
                        }
                        break;
                    case Keys.Up:
                        // P1 go up

                        if (player2.Position.Y + player2.Size.Y > 300)
                        {
                            tempVelPlayer2.Y = -(gameSpeed * MAX_SPEED);
                            player2.SetPlayerState(
                                Player.State.Climbing,
                                textures
                                );
                            resetTime2 = SPRITE_RESET_TIME_2;
                        }

                        if (!keyMap[Keys.Up] && player2.Velocity.Y == 0)
                        {
                            uint heldCoal = player2.HeldCoal;
                            bool success = MineTile(
                                ref heldCoal,
                                soundEffects,
                                player2,
                                ground,
                                new Vector2(
                                    player2.Position.X + player2.Size.X / 2,
                                    player2.Position.Y
                                    ),
                                new Vector2(
                                    player2.Size.X / 2,
                                    player2.Size.X / 2
                                    ),
                                ref rng
                                );
                            player2.HeldCoal = heldCoal;

                            if (success)
                            {
                                player2.SetPlayerState(
                                    Player.State.Mining_Up,
                                    textures
                                    );
                                resetTime2 = SPRITE_RESET_TIME_2;
                            }
                        }
                        break;
                    case Keys.Left:
                        tempVelPlayer2.X = -(gameSpeed * MAX_SPEED);

                        if (!keyMap[Keys.Left] && player2.Velocity.Y == 0)
                        {
                            uint heldCoal = player2.HeldCoal;
                            bool success = MineTile(
                                ref heldCoal,
                                soundEffects,
                                player2,
                                ground,
                                new Vector2(
                                    player2.Position.X,
                                    player2.Position.Y + player2.Size.Y / 2
                                    ),
                                new Vector2(
                                    player2.Size.X / 2,
                                    player2.Size.Y / 2
                                    ),
                                ref rng
                                );
                            player2.HeldCoal = heldCoal;

                            if (success)
                            {
                                player2.SetPlayerState(
                                    Player.State.Mining_Left,
                                    textures
                                    );
                                resetTime2 = SPRITE_RESET_TIME_2;
                            }
                        }
                        break;
                    case Keys.Right:
                        tempVelPlayer2.X = gameSpeed * MAX_SPEED;

                        if (!keyMap[Keys.Right] && player2.Velocity.Y == 0)
                        {
                            uint heldCoal = player2.HeldCoal;
                            bool success = MineTile(
                                ref heldCoal,
                                soundEffects,
                                player2,
                                ground,
                                new Vector2(
                                    player2.Position.X + player2.Size.X,
                                    player2.Position.Y + player2.Size.Y / 2
                                    ),
                                new Vector2(
                                    player2.Size.X / 2,
                                    player2.Size.Y / 2
                                    ),
                                ref rng
                                );
                            player2.HeldCoal = heldCoal;

                            if (success)
                            {
                                player2.SetPlayerState(
                                    Player.State.Mining_Right,
                                    textures
                                    );
                                resetTime2 = SPRITE_RESET_TIME_2;
                            }
                        }
                        break;
                    case Keys.Down:
                        // P2 dig down

                        // If key not held
                        if (!keyMap[Keys.Down] && player2.Velocity.Y == 0)
                        {
                            uint heldCoal = player2.HeldCoal;
                            bool success = MineTile(
                                ref heldCoal,
                                soundEffects,
                                player2,
                                ground,
                                new Vector2(
                                    player2.Position.X + player2.Size.X / 2,
                                    player2.Position.Y + player2.Size.Y
                                    ),
                                new Vector2(
                                    player2.Size.X / 2,
                                    player2.Size.X / 2
                                    ),
                                ref rng
                                );
                            player2.HeldCoal = heldCoal;

                            if (success)
                            {
                                player2.SetPlayerState(
                                    Player.State.Mining_Down,
                                    textures
                                    );
                                resetTime2 = SPRITE_RESET_TIME_2;
                            }
                        }
                        break;

                    case Keys.E:
                        if (!keyMap[Keys.E])
                        {
                            if (player1.Position.X + player1.Size.X / 2 < 100) // If P1 near house
                            {
                                state = State.GameState.DayEnd_Load;
                            }
                            else if (player1.Position.X + player1.Size.X / 2 < 200) // If P1 near coal deposit
                            {
                                totalCoal += (int)player1.HeldCoal;
                                player1.HeldCoal = 0;
                            }
                        }
                        break;
                    case Keys.RightControl:
                        if (!keyMap[Keys.RightControl])
                        {
                            if (player2.Position.X + player2.Size.X / 2 < 100) // If P2 near house
                            {
                                state = State.GameState.DayEnd_Load;
                            }
                            else if (player2.Position.X + player2.Size.X / 2 < 200) // If P2 near coal deposit
                            {
                                totalCoal += (int)player2.HeldCoal;
                                player2.HeldCoal = 0;
                            }
                        }
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
                lowestGround,
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

            // Draw snow background (technically wrong draw order, but it's visually unnoticeable
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
                textures,
                false
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
            if ((int)(timeLeft % 60) >= 10)
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

            // Draw interaction text
            if (player1.Position.X + player1.Size.X / 2 < 100) // If P1 near house
            {
                spriteBatch.DrawString(
                    fonts["SWTxt_12"],
                    "Press 'e' to end early",
                    new Vector2(
                        player1.Position.X * scale.X,
                        (player1.Position.Y - 25) * scale.Y
                        ),
                    Color.White,
                    0,
                    Vector2.Zero,
                    minScale,
                    0,
                    0
                    );
            }
            else if (player1.Position.X + player1.Size.X / 2 < 200) // If P1 near coal deposit
            {
                spriteBatch.DrawString(
                    fonts["SWTxt_12"],
                    "Press 'e' to deposit coal",
                    new Vector2(
                        player1.Position.X * scale.X,
                        (player1.Position.Y - 25) * scale.Y
                        ),
                    Color.White,
                    0,
                    Vector2.Zero,
                    minScale,
                    0,
                    0
                    );
            }
            if (player2.Position.X + player2.Size.X / 2 < 100) // If P2 near house
            {
                spriteBatch.DrawString(
                    fonts["SWTxt_12"],
                    "Press 'RCTRL' to end early",
                    new Vector2(
                        player2.Position.X * scale.X,
                        (player2.Position.Y - 12) * scale.Y
                        ),
                    Color.White,
                    0,
                    Vector2.Zero,
                    minScale,
                    0,
                    0
                    );
            }
            else if (player2.Position.X + player2.Size.X / 2 < 200) // If P2 near coal deposit
            {
                spriteBatch.DrawString(
                    fonts["SWTxt_12"],
                    "Press 'RCTRL' to deposit coal",
                    new Vector2(
                        player2.Position.X * scale.X,
                        (player2.Position.Y - 12) * scale.Y
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
   
            string iconName = $"{player.ID}_Icon";

            spriteBatch.Draw(
                textures[iconName],
                baseRect,
                Color.White
                );
            spriteBatch.DrawString(
                fonts["SWTxt_12"],
                $"Health: {Math.Round(player.Health, 0)}",
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
                $"Coal: {player.HeldCoal}/{player.CoalCapacity}",
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
                $"Temperature: {Math.Round(player.Temperature, 1)}C",
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
        private static bool MineTile(ref uint heldCoal, Dictionary<string, SoundEffect> soundEffects, Player player, Ground ground, Vector2 point, Vector2 range, ref Random rng)
        {

            (int y, int x) tile = Ground.GetNearestTileToPoint(
                point,
                ground,
                new Rectangle(
                    200,
                    300,
                    760,
                    1000
                    ),
                range
                );

            if (tile.y >= 0 && tile.x >= 0)
            {
                switch (ground.Tiles[tile.y, tile.x].Type)
                {
                    default: // Other
                        ground.Tiles[tile.y, tile.x].Filled = false;
                        break;
                    case 1: // Coal
                        heldCoal++;
                        if (player.CoalCapacity < heldCoal)
                        {
                            heldCoal = player.CoalCapacity;
                        }
                        ground.Tiles[tile.y, tile.x].Filled = false;
                        break;
                }
                soundEffects[$"Pick_Hit_{rng.Next(4)}"].Play();
                return true;
            }
            return false;
        }
    }
}
