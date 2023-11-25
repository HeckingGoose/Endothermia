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
        private const double SKIP_TIME = 4;

        // External Variables
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Internal Variables
        private double time;
        private string dayString;
        private Vector2 textSize;
        private Vector2 textPosition;


        // Constructor
        public Day_Screen(
            ref GraphicsDeviceManager graphics,
            ref SpriteBatch spriteBatch,
            Dictionary<string, SpriteFont> fonts,
            int day
            )
        {
            // Assign values
            _graphics = graphics;
            _spriteBatch = spriteBatch;

            // Set starting value
            time = 0;

            // Define dayString and calculate its position
            dayString = $"Day {day}";
            textSize = fonts["SWTxt_48"].MeasureString(dayString);
            textPosition = new Vector2(480 - (textSize.X/2), 270 - (textSize.Y/2));
            
        }

        // Methods
        public void RunLogic(
            double gameTime,
            ref State.GameState state
            )
        {
            // Count how long this scene has been active
            time += gameTime;
            
            // If the scene is older than SKIP_TIME
            if (time > SKIP_TIME)
            {
                // Move into the game loading state
                state = State.GameState.Game_Load;
            }

        }
        public void RunGraphics(
            SpriteBatch spriteBatch,
            Dictionary<string, SpriteFont> fonts
            )
        {
            // Draw dayString
            spriteBatch.DrawString(
                fonts["SWTxt_48"],
                dayString,
                textPosition,
                Color.White
                );
        }
    }
}
