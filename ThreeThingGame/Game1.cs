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
        // Window scale tracking
        private Vector2 scale;

        // Screens
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

            // Initialise screens
            menuScreen = new Menu(ref _graphics, ref _spriteBatch);

            // Load screen content
            menuScreen.Load();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            menuScreen.RunLogic();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            menuScreen.RunGraphics(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
