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
        List<Button> buttons = new List<Button>();


        // Constructor
        public Menu(ref GraphicsDeviceManager graphics, ref SpriteBatch spriteBatch, Texture2D texture)
        {
            _graphics = graphics;
            _spriteBatch = spriteBatch;
            Button newGame = new Button(
                "NEW GAME", 
                false, 
                texture, 
                new Rectangle(325, 300, 250, 100), 
                NewGameStart
                );
            buttons.Add(newGame);
            Button exitGame = new Button(
                "X",
                false,
                texture,
                new Rectangle(935, 0, 25, 25),
                ExitGame
                );
            buttons.Add(exitGame);
        }

        // Methods
        public void RunLogic(ref uint state)
        {
            // Run logic here
        }
        public void RunGraphics(SpriteBatch spriteBatch,
            SpriteFont SWTxt_36,
            Texture2D titleTexture
            )
        {
            spriteBatch.DrawString(SWTxt_36, "Test text", new Vector2(10,10), Color.White);

            spriteBatch.Draw(titleTexture, new Rectangle(320, 160, 400, 300), Color.White);
        }

        private void NewGameStart(ref uint state)
        {
            state = 1;
        }

        private void ExitGame(ref uint state)
        {
            Exit();
        }
    }
}
