using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

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
        List<Button> buttons = new List<Button>();
        Player player1;
        Player player2;
        
        // Constructor
        public Game_Screen(ref GraphicsDeviceManager graphics, ref SpriteBatch spriteBatch)
        {
            _graphics = graphics;
            _spriteBatch = spriteBatch;
            player1 = new Player(
                0
                );
            player2 = new Player(
                1
                );
        }

        // Methods
        public void RunLogic(float gameSpeed)
        {
            // Run logic here
            Vector2 tempVelPlayer1 = player1.Velocity;
            Vector2 tempVelPlayer2 = player2.Velocity;
            foreach (Keys key in Keyboard.GetState().GetPressedKeys())
            {
                switch (key)
                {
                    case Keys.A:
                        tempVelPlayer1.X = -(gameSpeed * MOVESPEED);
                        break;
                    case Keys.S:
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

        }
        public void RunGraphics(
            SpriteBatch spriteBatch
            )
        {
            foreach (Button button in buttons)
            {
                spriteBatch.Draw(button.Texture, button.Rect, button.BackColour);
                spriteBatch.DrawString(button.Font, button.Text, new Vector2(button.Rect.X, button.Rect.Y), button.TextColour);
            }
        }
    }
}
