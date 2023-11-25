using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ThreeThingGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // CONST
        private const int TARGET_FRAMERATE = 60;
        private const int TARGET_WIDTH = 960;
        private const int TARGET_HEIGHT = 540;
        // ENDCONST

        // VAR
        // Fonts
        private SpriteFont SWTxt_36;

        // Window scale tracking
        private Vector2 scale;

        // Screens
        private uint state;
        private Menu menuScreen;

        // ENDVAR

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Initialise values
            state = 0;

            // Initialise window size
            _graphics.PreferredBackBufferWidth = TARGET_WIDTH;
            _graphics.PreferredBackBufferHeight = TARGET_HEIGHT;

            // Set scale
            scale.X = _graphics.PreferredBackBufferWidth / TARGET_WIDTH;
            scale.Y = _graphics.PreferredBackBufferHeight / TARGET_HEIGHT;

            // Apply changes
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Initialise menu screen
            menuScreen = new Menu(ref _graphics, ref _spriteBatch);

            // Load game content
            SWTxt_36 = Content.Load<SpriteFont>(@"Fonts\SWTxt_36");
        }

        protected override void Update(GameTime gameTime)
        {
            switch (state)
            {
                case 0: // Menu screen
                    menuScreen.RunLogic(
                        );
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Begin draw
            _spriteBatch.Begin();

            switch (state)
            {
                case 0: // Menu screen
                    menuScreen.RunGraphics(
                        _spriteBatch,
                        SWTxt_36
                        );
                    break;
            }

            /// End draw
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
