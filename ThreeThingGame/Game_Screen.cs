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
    internal class Game_Screen : Game
    {
        // Constants
        private const float MOVESPEED = 1;

        // External Variables
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Internal Variables
        private Ground ground;
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

            // Create ground
            ground = new Ground(
                Ground.GenerateGround(
                    groundWidth,
                    groundDepth,
                    coalDist,
                    oilDist,
                    gasDist,
                    clumpSum
                    ),
                groundWidth,
                groundDepth
                );

            // Initialise camera position
            cameraPosition = Vector2.Zero;
        }

        // Methods
        public void RunLogic(float gameSpeed, Keys[] KeysPressed)
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
                    case Keys.S:
                        tempVelPlayer1.Y = (gameSpeed * MOVESPEED);
                        break;

                }
            }
            player1.Velocity = tempVelPlayer1;
            player2.Velocity = tempVelPlayer2;

            player1.Position += player1.Velocity;
            player2.Position += player2.Velocity;

            cameraPosition.Y = (player1.Position.Y + player2.Position.Y) / 2;

        }
        public void RunGraphics(
            SpriteBatch spriteBatch,
            Dictionary<string, Texture2D> textures
            )
        {

            // Draw test ground
            Ground.DrawGround(
                spriteBatch,
                ground.Tiles,
                ground.Width,
                ground.Depth,
                new Rectangle(200, 300 - (int)cameraPosition.Y, 760, 1000),
                textures
                );
        }
    }
}
