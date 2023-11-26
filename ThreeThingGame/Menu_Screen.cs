using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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

            // Cache buttons
            buttons.Add(newGame);
        }

        // Methods
        public void RunLogic(
            ref State.GameState state,
            bool[] mouseButtonsHeld,
            MouseState mouseState,
            Vector2 scale
            )
        {
            foreach(Button button in buttons)
            {
                if (mouseState.Position.X >= (button.Rect.X) * scale.X
                    && mouseState.Position.X <= (button.Rect.X + button.Rect.Width) * scale.Y
                    && mouseState.Position.Y >= (button.Rect.Y) * scale.X
                    && mouseState.Position.Y <= (button.Rect.Y + button.Rect.Height) * scale.Y
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
            Vector2 scale,
            Dictionary<string, Texture2D> textures
            )
        {
            // Draw sprite
            spriteBatch.Draw(
                textures["TitleTexture"],
                new Rectangle(
                    (int)(280 * scale.X),
                    (int)(80 * scale.Y),
                    (int)(400 * scale.X),
                    (int)(300 * scale.Y)
                    ),
                Color.White
                );
            
            // Draw buttons
            foreach(Button button in buttons )
            {
                spriteBatch.Draw(
                    button.Texture,
                    new Rectangle(
                        (int)(button.Rect.X * scale.X),
                        (int)(button.Rect.Y * scale.Y),
                        (int)(button.Rect.Width * scale.X),
                        (int)(button.Rect.Height * scale.Y)
                        ),
                    button.BackColour);
                spriteBatch.DrawString(
                    button.Font,
                    button.Text,
                    button.TextPosition * scale,
                    button.TextColour,
                    0,
                    Vector2.Zero,
                    Math.Min(scale.X, scale.Y),
                    0,
                    0
                    );
            }
        }

        private void NewGameStart(ref State.GameState state)
        {
            state = State.GameState.Intro_Load;//State.GameState.Intro_Load;
        }
    }
}
