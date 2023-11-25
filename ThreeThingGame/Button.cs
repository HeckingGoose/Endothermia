using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeThingGame
{
    internal class Button
    {
        // Variables
        private string _text;
        private Vector2 _textPosition;
        private bool _wordWrap;
        private Color _textColour;
        private Color _backColour;
        private SpriteFont _font;
        private Texture2D _texture;
        private Rectangle _rect;
        private ClickEvent _onClick; 
        // Callback method
        public delegate void ClickEvent(ref State.GameState state);

        // Constructors
        public Button()
        {
            _text = "";
            _wordWrap = false;
            _textColour = Color.Black;
            _backColour = Color.White;
            _font = null;
            _texture = null;
            _rect = new Rectangle();
            _onClick = null;
            _textPosition = new Vector2();
        }
        public Button(
            string text,
            bool wordWrap,
            Color textColour,
            Color backColour,
            SpriteFont font,
            Texture2D texture,
            Rectangle rect,
            ClickEvent onClick
            )
        {
            _text = text;
            _wordWrap = wordWrap;
            _textColour = textColour;
            _backColour = backColour;
            _font = font;
            _texture = texture;
            _rect = rect;
            _onClick = onClick;

            // Calculate text position
            _textPosition = CalculateTextPosition(
                _rect,
                _font,
                _text
                );
        }

        // Static methods
        private static Vector2 CalculateTextPosition(Rectangle rect, SpriteFont font, string text)
        {
            // Define new vector
            Vector2 output = Vector2.Zero;

            // Calculate size of text in button
            Vector2 textSize = font.MeasureString(text);

            // Calculate text position based on button rect and text size
            output = new Vector2(
                rect.X + (rect.Width - textSize.X) / 2,
                rect.Y + (rect.Height - textSize.Y) / 2
                );

            // Return result
            return output;
        }

        // Methods
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
        public SpriteFont Font
        {
            get { return _font; }
            set { _font = value; }
        }
        public bool WordWrap
        {
            get { return _wordWrap; }
            set { _wordWrap = value; }
        }
        public Color TextColour
        {
            get { return _textColour; }
            set { _textColour = value; }
        }
        public Color BackColour
        {
            get { return _backColour; }
            set { _backColour = value; }
        }
        public Texture2D Texture
        {
            get { return _texture; }
            set { _texture = value; }
        }
        public Rectangle Rect
        {
            get { return _rect; }
            set { _rect = value; }
        }
        public ClickEvent OnClick
        {
            get { return _onClick; }
            set { _onClick = value; }
        }
        public Vector2 TextPosition
        {
            get { return _textPosition; }
            set { _textPosition = value; }
        }
    }
}
