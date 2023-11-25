using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeThingGame
{
    internal class Day_Screen
    {
        // Constants
        private const double Skip_Time = 4;

        // External Variables
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Internal Variables
        private double time;
        private Vector2 textSize;
        private string dayString;
        private Vector2 textPosition;


        // Constructor
        public Day_Screen(ref GraphicsDeviceManager graphics, ref SpriteBatch spriteBatch, Dictionary<string, SpriteFont> fonts, int day)
        {
            _graphics = graphics;
            _spriteBatch = spriteBatch;
            time = 0;
            dayString = $"Day {day}";
            textSize = fonts["SWTxt_48"].MeasureString(dayString);
            textPosition = new Vector2(480 - (textSize.X/2), 270 - (textSize.Y/2));
            
        }

        // Methods
        public void RunLogic(double gameTime, ref uint state)
        {
            // Run logic here
            time += gameTime;
            if (time > Skip_Time)
            {
                state = 5;
            }

        }
        public void RunGraphics(
            SpriteBatch spriteBatch,
            Dictionary<string, SpriteFont> fonts
            )
        {
            spriteBatch.DrawString(fonts["SWTxt_48"], dayString, textPosition, Color.White);
        }
    }
}
