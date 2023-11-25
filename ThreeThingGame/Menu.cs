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
        private SpriteFont font;

        // Constructor
        public Menu(ref GraphicsDeviceManager graphics, ref SpriteBatch spriteBatch)
        {
            _graphics = graphics;
            _spriteBatch = spriteBatch;
            Content.RootDirectory = "Content";

            base.Initialize();
        }

        // Methods
        public  void Load()
        {

            SpriteFont font = Content.Load<SpriteFont>(@"Fonts\File");
        }
        public void RunLogic()
        {

        }
        public void RunGraphics(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, "Test text", new Vector2(10,10), Color.White);
        }
    }
}
