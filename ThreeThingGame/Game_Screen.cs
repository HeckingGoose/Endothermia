using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace ThreeThingGame
{
    internal class Game_Screen
    {
        // Constants
        private const float MOVESPEED = 1;

        // External Variables
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Internal Variables
        // General
        private double timeLeft;
        private Ground ground;
        private Ground unplayableGround;
        private Vector2 cameraPosition;

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
                0
                );
            player2 = new Player(
                1
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

            // Initialise camera position
            cameraPosition = Vector2.Zero;

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
            float gameSpeed,
            double deltaTime,
            Keys[] KeysPressed,
            Vector2 scale
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
                        tempVelPlayer1.X = -(gameSpeed * MOVESPEED);
                        break;
                    case Keys.D:
                        tempVelPlayer1.X = gameSpeed * MOVESPEED;
                        break;
                    case Keys.Left:
                        tempVelPlayer2.X = -(gameSpeed * MOVESPEED);
                        break;
                    case Keys.Right:
                        tempVelPlayer2.X = gameSpeed * MOVESPEED;
                        break;

                }
            }

            // Set player velocities
            player1.Velocity = tempVelPlayer1;
            player2.Velocity = tempVelPlayer2;

            // Apply player velocities
            player1.Position += player1.Velocity;
            player2.Position += player2.Velocity;

            // Reset player velocities
            player1.Velocity = Vector2.Zero;
            player2.Velocity = Vector2.Zero;

            // Calculate new camera Y position
            cameraPosition.Y = (player1.Position.Y + player2.Position.Y) / 2;

        }
        public void RunGraphics(
            SpriteBatch spriteBatch,
            Vector2 scale,
            Dictionary<string, Texture2D> textures,
            Dictionary<string, SpriteFont> fonts
            )
        {
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

            // Draw timer
            spriteBatch.DrawString(
                fonts["SWTxt_24"],
                timeLeftPhrase,
                timeLeftPos * scale,
                Color.White,
                0,
                Vector2.Zero,
                Math.Min(scale.X, scale.Y),
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
                Math.Min(scale.X, scale.Y),
                0,
                0
                );
        }
    }
}
