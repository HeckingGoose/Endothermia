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
        public Menu(
            ref GraphicsDeviceManager graphics,
            ref SpriteBatch spriteBatch,
            Texture2D buttonTexture,
            Dictionary<string, SpriteFont> fonts
            )
        {
            _graphics = graphics;
            _spriteBatch = spriteBatch;
            Button newGame = new Button(
                text: "NEW GAME",
                wordWrap: false,
                font: fonts["SWTxt_24"],
                textColour: Color.Black,
                backColour: Color.White,
                texture: buttonTexture,
                rect: new Rectangle(355, 420, 250, 50),
                onClick: NewGameStart
                );
            buttons.Add(newGame);
            Button exitGame = new Button(
                text: "X",
                wordWrap: false,
                font: fonts["SWTxt_12"],
                textColour: Color.White,
                backColour: new Color(255, 200, 200, 255),
                texture: buttonTexture,
                rect: new Rectangle(935, 0, 25, 25),
                onClick: ExitGame
                );
            buttons.Add(exitGame);
        }

        // Methods
        public void RunLogic(
            ref uint state,
            bool[] mouseButtonsHeld,
            MouseState mouseState,
            (int X, int Y) mousePosition
            )
        {

            // Run logic here
            foreach(Button button in buttons)
            {
                if (mousePosition.X >= button.Rect.X 
                    && mousePosition.X <= button.Rect.X + button.Rect.Width
                    && mousePosition.Y >= button.Rect.Y
                    && mousePosition.Y <= button.Rect.Y + button.Rect.Height
                    )
                {
                    if (!mouseButtonsHeld[0] 
                        && mouseState.LeftButton == ButtonState.Pressed)
                    {
                        button.OnClick(ref state);
                    }
                }
            }
        }
        public void RunGraphics(
            SpriteBatch spriteBatch,
            Texture2D titleTexture
            )
        {
            spriteBatch.Draw(titleTexture, new Rectangle(280, 80, 400, 300), Color.White);
            foreach(Button button in buttons )
            {
                spriteBatch.Draw(button.Texture, button.Rect, button.BackColour);
                spriteBatch.DrawString(button.Font, button.Text, new Vector2(button.Rect.X, button.Rect.Y), button.TextColour);
            }
        }

        private void NewGameStart(ref uint state)
        {
            state = 1;
        }

        private void ExitGame(ref uint state)
        {
            Exit();
            Environment.Exit(0);
        }
    }
}
