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
        private Ground ground;
        private Ground unplayableGround;
        private Vector2 cameraPosition;
        private List<Button> buttons = new List<Button>();
        private Player player1;
        private Player player2;
        
        // Constructor
        public Game_Screen(
            ref GraphicsDeviceManager graphics,
            ref SpriteBatch spriteBatch,
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
        }

        // Methods
        public void RunLogic(
            float gameSpeed,
            Keys[] KeysPressed,
            Vector2 scale
            )
        {
            // Run logic here
            Vector2 tempVelPlayer1 = player1.Velocity;
            Vector2 tempVelPlayer2 = player2.Velocity;
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
            player1.Velocity = tempVelPlayer1;
            player2.Velocity = tempVelPlayer2;

            player1.Position += player1.Velocity;
            player2.Position += player2.Velocity;

            player1.Velocity = Vector2.Zero;
            player2.Velocity = Vector2.Zero;

            cameraPosition.Y = (player1.Position.Y + player2.Position.Y) / 2;

        }
        public void RunGraphics(
            SpriteBatch spriteBatch,
            Vector2 scale,
            Dictionary<string, Texture2D> textures
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
        }
    }
}
