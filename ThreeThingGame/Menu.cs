using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeThingGame
{
    public class Menu : Game
    {
        // External Variables
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Internal Variables


        // Constructor
        public Menu(ref GraphicsDeviceManager graphics, ref SpriteBatch spriteBatch)
        {
            _graphics = graphics;
            _spriteBatch = spriteBatch;
        }

        // Methods
        public void RunLogic()
        {
            // Run logic here
        }
        public void RunGraphics(SpriteBatch spriteBatch,
            SpriteFont SWTxt_36)
        {
            spriteBatch.DrawString(SWTxt_36, "Test text", new Vector2(10,10), Color.White);
        }
    }
}
