using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

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
        private const int DAYTIME_SECONDS = 240;
        // ENDCONST

        // VAR
        // Fonts
        private Dictionary<string, SpriteFont> fonts;

        // Textures
        private Dictionary<string, Texture2D> textures;

        // Window scale tracking
        private Vector2 scale;

        // Screens
        private State.GameState state;
        private Menu_Screen menuScreen;
        private Intro_Screen introScreen;
        private Game_Screen gameScreen;
        private Day_Screen dayScreen;

        // Mouse held
        private bool[] mouseButtonsHeld;

        // Time tracking
        private int day;

        // Keymap
        private Dictionary<Keys, bool> keyMap;

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
            state = State.GameState.Menu_Main;
            fonts = new Dictionary<string, SpriteFont>();
            textures = new Dictionary<string, Texture2D>();
            keyMap = new Dictionary<Keys, bool>();
            mouseButtonsHeld = new bool[3];
            day = 1;

            // Track utility keys
            keyMap.Add(Keys.F11, false);

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

            // Load game content
            fonts.Add("SWTxt_12", Content.Load<SpriteFont>(@"Fonts\SWTxt_12"));
            fonts.Add("SWTxt_24", Content.Load<SpriteFont>(@"Fonts\SWTxt_24"));
            fonts.Add("SWTxt_36", Content.Load<SpriteFont>(@"Fonts\SWTxt_36"));
            fonts.Add("SWTxt_48", Content.Load<SpriteFont>(@"Fonts\SWTxt_48"));

            // Title screen
            textures.Add("ButtonTexture", Content.Load<Texture2D>(@"Sprites\ButtonTexture"));
            textures.Add("TitleTexture", Content.Load<Texture2D>(@"Sprites\blackTitle"));

            // Gameplay
            textures.Add("Snow", Content.Load<Texture2D>(@"Sprites\snow"));
            textures.Add("Coal", Content.Load<Texture2D>(@"Sprites\coal"));
            textures.Add("Oil", Content.Load<Texture2D>(@"Sprites\oilFull"));
            textures.Add("Gas", Content.Load<Texture2D>(@"Sprites\gas"));
            textures.Add("Rock", Content.Load<Texture2D>(@"Sprites\rock"));
            textures.Add("Bedrock", Content.Load<Texture2D>(@"Sprites\bedrock"));
            textures.Add("Empty", Content.Load<Texture2D>(@"Sprites\empty"));
            textures.Add("GameplayBase", Content.Load<Texture2D>(@"Sprites\gameplayBase"));

            // UI
            textures.Add("Blue_Icon", Content.Load<Texture2D>(@"Sprites\blue_icon"));
            textures.Add("Red_Icon", Content.Load<Texture2D>(@"Sprites\red_icon"));

            // Player sprites
            textures.Add("Blue_Front", Content.Load<Texture2D>(@"Sprites\blue_front"));
            textures.Add("Red_Front", Content.Load<Texture2D>(@"Sprites\red_front"));


            // Initialise menu screen
            menuScreen = new Menu_Screen(
                ref _graphics,
                ref _spriteBatch,
                textures,
                fonts
                );
        }

        protected override void Update(GameTime gameTime)
        {
            // Calculate game speed
            float gameSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * TARGET_FRAMERATE;

            // Get mouse state
            MouseState mouseState = Mouse.GetState();

            // Get keyboard state
            KeyboardState keyboardState = Keyboard.GetState();

            // Toggle fullscreen
            if (keyboardState.IsKeyDown(Keys.F11) && !keyMap[Keys.F11])
            {
                // Set F11 to be pressed
                keyMap[Keys.F11] = true;

                // Flip display mode
                _graphics.IsFullScreen = !_graphics.IsFullScreen;

                if (_graphics.IsFullScreen)
                {
                    // Switch resolution to match monitor
                    _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                    _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

                    // Set game window to borderless
                    _graphics.HardwareModeSwitch = false;

                    // Set scale
                    scale.X = _graphics.PreferredBackBufferWidth / TARGET_WIDTH;
                    scale.Y = _graphics.PreferredBackBufferHeight / TARGET_HEIGHT;
                }
                else
                {
                    // Switch resolution to meet target
                    _graphics.PreferredBackBufferWidth = TARGET_WIDTH;
                    _graphics.PreferredBackBufferHeight = TARGET_HEIGHT;

                    // Set game window to windowed
                    _graphics.HardwareModeSwitch = true;

                    // Set scale
                    scale.X = _graphics.PreferredBackBufferWidth / TARGET_WIDTH;
                    scale.Y = _graphics.PreferredBackBufferHeight / TARGET_HEIGHT;
                }

                _graphics.ApplyChanges();
            }
            else if (!keyboardState.IsKeyDown(Keys.F11) && keyMap[Keys.F11])
            {
                keyMap[Keys.F11] = false;
            }

            switch (state)
            {
                case State.GameState.Menu_Main:
                    menuScreen.RunLogic(
                        ref state,
                        mouseButtonsHeld,
                        mouseState,
                        scale
                        );
                    break;

                case State.GameState.Intro_Load:
                    introScreen = new Intro_Screen(
                        ref _graphics,
                        ref _spriteBatch
                        );
                    state = State.GameState.Intro_Main;
                    break;

                case State.GameState.Intro_Main:
                    introScreen.RunLogic(
                        );
                    break;

                case State.GameState.Day_Load:
                    dayScreen = new Day_Screen(
                        ref _graphics,
                        ref _spriteBatch,
                        fonts,
                        day
                        );
                    state = State.GameState.Day_Main;
                    break;

                case State.GameState.Day_Main:
                    dayScreen.RunLogic(
                        gameTime.ElapsedGameTime.TotalSeconds,
                        ref state
                        );
                    break;

                case State.GameState.Game_Load:
                    gameScreen = new Game_Screen(
                        ref _graphics,
                        ref _spriteBatch,
                        DAYTIME_SECONDS,
                        fonts,
                        textures,
                        19,
                        25,
                        0.05f,
                        0.025f,
                        0.015f,
                        0.2f
                        );
                    state = State.GameState.Game_Main;
                    break;

                case State.GameState.Game_Main:
                    gameScreen.RunLogic(
                        ref state,
                        gameSpeed,
                        gameTime.ElapsedGameTime.TotalSeconds,
                        keyboardState.GetPressedKeys()
                        );
                    break;

                case State.GameState.DayEnd_Load:
                    break;

                case State.GameState.DayEnd_Main:
                    // Increase day
                    break;

            }

            // Update if mouse is held
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                mouseButtonsHeld[0] = true;
            }
            else
            {
                mouseButtonsHeld[0] = false;
            }

            if (mouseState.RightButton == ButtonState.Pressed)
            {
                mouseButtonsHeld[1] = true;
            }
            else
            {
                mouseButtonsHeld[1] = false;
            }

            if (mouseState.MiddleButton == ButtonState.Pressed)
            {
                mouseButtonsHeld[2] = true;
            }
            else
            {
                mouseButtonsHeld[2] = false;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Begin draw
            _spriteBatch.Begin();

            // Draw current screen
            switch (state)
            {
                case State.GameState.Menu_Main:
                    GraphicsDevice.Clear(Color.CornflowerBlue);
                    menuScreen.RunGraphics(
                        _spriteBatch,
                        scale,
                        textures
                        );
                    break;

                case State.GameState.Day_Main:
                    GraphicsDevice.Clear(Color.Black);
                    dayScreen.RunGraphics(
                        _spriteBatch,
                        scale,
                        fonts
                        );
                    break;

                case State.GameState.Intro_Main:
                    GraphicsDevice.Clear(Color.CornflowerBlue);
                    introScreen.RunGraphics(
                        _spriteBatch
                        );
                    break;

                case State.GameState.Game_Main:
                    GraphicsDevice.Clear(Color.CornflowerBlue);
                    gameScreen.RunGraphics(
                        _spriteBatch,
                        scale,
                        textures,
                        fonts
                        );
                    break;
                case State.GameState.DayEnd_Main:
                    GraphicsDevice.Clear(Color.CornflowerBlue);
                    break;
            }

            /// End draw
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
