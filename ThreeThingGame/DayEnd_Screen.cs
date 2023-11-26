using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeThingGame
{
    internal class DayEnd_Screen
    {
        // External Variables
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Internal Variables
        List<Button> buttons = new List<Button>();


        // Constructor
        public DayEnd_Screen(
            ref GraphicsDeviceManager graphics,
            ref SpriteBatch spriteBatch,
            Dictionary<string, SpriteFont> fonts,
            Dictionary<string, Texture2D> textures
            )
        {
            _graphics = graphics;
            _spriteBatch = spriteBatch;

            Button continueButton = new Button(
                "Continue",
                false,
                Color.Black,
                Color.White,
                fonts["SWTxt_24"],
                textures["ButtonTexture"],
                new Rectangle(
                    740,
                    470,
                    200,
                    50
                    ),
                GoToDay
                );

            buttons.Add(continueButton);
        }

        // Methods
        public void RunLogic(
            ref State.GameState state,
            bool[] mouseButtonsHeld,
            MouseState mouseState,
            Vector2 scale
            )
        {
            foreach (Button button in buttons)
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
            Dictionary<string, SpriteFont> fonts,
            Vector2 scale
            )
        {
            // Draw buttons
            foreach (Button button in buttons)
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
        private void GoToDay(ref State.GameState state)
        {
            state = State.GameState.Day_Load;
        }
    }
}
