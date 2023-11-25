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
    internal class Menu_Screen
    {
        // External Variables
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Internal Variables
        List<Button> buttons = new List<Button>();


        // Constructor
        public Menu_Screen(
            ref GraphicsDeviceManager graphics,
            ref SpriteBatch spriteBatch,
            Dictionary<string, Texture2D> textures,
            Dictionary<string, SpriteFont> fonts
            )
        {
            // Assign values
            _graphics = graphics;
            _spriteBatch = spriteBatch;

            // Create new game button
            Button newGame = new Button(
                text: "NEW GAME",
                wordWrap: false,
                font: fonts["SWTxt_24"],
                textColour: Color.Black,
                backColour: Color.White,
                texture: textures["ButtonTexture"],
                rect: new Rectangle(355, 420, 250, 50),
                onClick: NewGameStart
                );

            // Create exit game button
            Button exitGame = new Button(
                text: "X",
                wordWrap: false,
                font: fonts["SWTxt_12"],
                textColour: Color.White,
                backColour: new Color(150, 50, 50, 255),
                texture: textures["ButtonTexture"],
                rect: new Rectangle(935, 0, 25, 25),
                onClick: ExitGame
                );

            // Cache buttons
            buttons.Add(newGame);
            buttons.Add(exitGame);
        }

        // Methods
        public void RunLogic(
            ref State.GameState state,
            bool[] mouseButtonsHeld,
            MouseState mouseState
            )
        {

            // Run logic here
            foreach(Button button in buttons)
            {
                if (mouseState.Position.X >= button.Rect.X 
                    && mouseState.Position.X <= button.Rect.X + button.Rect.Width
                    && mouseState.Position.Y >= button.Rect.Y
                    && mouseState.Position.Y <= button.Rect.Y + button.Rect.Height
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
            Dictionary<string, Texture2D> textures
            )
        {
            spriteBatch.Draw(textures["TitleTexture"], new Rectangle(280, 80, 400, 300), Color.White);
            foreach(Button button in buttons )
            {
                spriteBatch.Draw(button.Texture, button.Rect, button.BackColour);
                spriteBatch.DrawString(button.Font, button.Text, button.TextPosition, button.TextColour);
            }
        }

        private void NewGameStart(ref State.GameState state)
        {
            state = State.GameState.Intro_Load;
        }

        private void ExitGame(ref State.GameState state)
        {
            Environment.Exit(0);
        }
    }
}
