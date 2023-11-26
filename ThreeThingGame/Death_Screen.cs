using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace ThreeThingGame
{
    internal class Death_Screen
    {
        // External Variables
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Internal Variables
        List<Button> buttons = new List<Button>();


        // Constructor
        public Death_Screen(
            ref GraphicsDeviceManager graphics,
            ref SpriteBatch spriteBatch,
            Dictionary<string, SpriteFont> fonts,
            Dictionary<string, Texture2D> textures
            )
        {
            Button quitButton = new Button(
                "Quit",
                false,
                Color.Black,
                Color.White,
                fonts["SWTxt_24"],
                textures["ButtonTexture"],
                new Rectangle(
                    430,
                    375,
                    100,
                    50
                    ),
                Quit
                );
            buttons.Add(quitButton);
        }
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
            Vector2 scale,
            string deathText
            )
        {
            float minScale = Math.Min(scale.X, scale.Y);

            Vector2 length = fonts["SWTxt_48"].MeasureString(deathText);

            // Draw some text
            spriteBatch.DrawString(
                fonts["SWTxt_48"],
                deathText,
                new Vector2(
                    480 - length.X / 2,
                    200
                ),
                Color.White,
                0,
                Vector2.Zero,
                minScale,
                0,
                0
                );

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
            private void Quit(ref State.GameState state)
        {
            Environment.Exit(0);
        }
    }
}
