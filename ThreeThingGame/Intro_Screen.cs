using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeThingGame
{
    internal class Intro_Screen
    {
        // External Variables
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Internal Variables
        List<Button> buttons = new List<Button>();


        // Constructor
        public Intro_Screen(ref GraphicsDeviceManager graphics, ref SpriteBatch spriteBatch)
        {
            _graphics = graphics;
            _spriteBatch = spriteBatch;
        }

        // Methods
        public void RunLogic()
        {
            // Run logic here
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
