using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
        private const int DAYTIME_SECONDS = 120;
        // ENDCONST

        // VAR
        // Fonts
        private Dictionary<string, SpriteFont> fonts;

        // Textures
        private Dictionary<string, Texture2D> textures;

        // Sound effects
        private Dictionary<string, SoundEffect> soundEffects;

        // Window scale tracking
        private Vector2 scale;
        private string deathText = "You lose!";

        // Screens
        private State.GameState state;
        private Menu_Screen menuScreen;
        private Intro_Screen introScreen;
        private Game_Screen gameScreen;
        private Day_Screen dayScreen;
        private DayEnd_Screen dayEndScreen;
        private Death_Screen deathScreen;

        // Mouse held
        private bool[] mouseButtonsHeld;

        // Time tracking
        private int day;

        // Keymap
        private Dictionary<Keys, bool> keyMap;

        // Persistent game variables
        private int totalCoal;
        private int coalQuota;
        private int heatingCost;

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
            soundEffects = new Dictionary<string, SoundEffect>();
            keyMap = new Dictionary<Keys, bool>();
            mouseButtonsHeld = new bool[3];
            day = 0;
            totalCoal = 0;
            coalQuota = 15;
            heatingCost = 5;

            // Track utility keys
            keyMap.Add(Keys.F11, false);
            keyMap.Add(Keys.W, false);
            keyMap.Add(Keys.A, false);
            keyMap.Add(Keys.S, false);
            keyMap.Add(Keys.D, false);
            keyMap.Add(Keys.Up, false);
            keyMap.Add(Keys.Left, false);
            keyMap.Add(Keys.Down, false);
            keyMap.Add(Keys.Right, false);
            keyMap.Add(Keys.E, false);
            keyMap.Add(Keys.RightControl, false);

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
            textures.Add("Snow_Overlay", Content.Load<Texture2D>(@"Sprites\snow_overlay"));

            // UI
            textures.Add("Blue_Icon", Content.Load<Texture2D>(@"Sprites\blue_icon"));
            textures.Add("Red_Icon", Content.Load<Texture2D>(@"Sprites\red_icon"));

            // Player sprites
            textures.Add("Blue_Front", Content.Load<Texture2D>(@"Sprites\blue_front"));
            textures.Add("Blue_Mine_Right", Content.Load<Texture2D>(@"Sprites\blue_mine_right"));
            textures.Add("Blue_Mine_Left", Content.Load<Texture2D>(@"Sprites\blue_mine_left"));

            textures.Add("Red_Front", Content.Load<Texture2D>(@"Sprites\red_front"));
            textures.Add("Red_Mine_Right", Content.Load<Texture2D>(@"Sprites\red_mine_right"));
            textures.Add("Red_Mine_Left", Content.Load<Texture2D>(@"Sprites\red_mine_left"));

            textures.Add("Red_Climb", Content.Load<Texture2D>(@"Sprites\red_front"));
            textures.Add("Red_Mine_Down", Content.Load<Texture2D>(@"Sprites\red_front"));
            textures.Add("Red_Mine_Up", Content.Load<Texture2D>(@"Sprites\red_front"));
            textures.Add("Blue_Climb", Content.Load<Texture2D>(@"Sprites\blue_front"));
            textures.Add("Blue_Mine_Down", Content.Load<Texture2D>(@"Sprites\blue_front"));
            textures.Add("Blue_Mine_Up", Content.Load<Texture2D>(@"Sprites\blue_front"));

            // Sound effects
            soundEffects.Add("Pick_Hit_0", Content.Load<SoundEffect>(@"Audio\pick_hit_0"));
            soundEffects.Add("Pick_Hit_1", Content.Load<SoundEffect>(@"Audio\pick_hit_1"));
            soundEffects.Add("Pick_Hit_2", Content.Load<SoundEffect>(@"Audio\pick_hit_2"));
            soundEffects.Add("Pick_Hit_3", Content.Load<SoundEffect>(@"Audio\pick_hit_3"));

            soundEffects.Add("Snow_Walk_0", Content.Load<SoundEffect>(@"Audio\snow_walk_0"));
            soundEffects.Add("Snow_Walk_1", Content.Load<SoundEffect>(@"Audio\snow_walk_1"));
            soundEffects.Add("Snow_Walk_2", Content.Load<SoundEffect>(@"Audio\snow_walk_2"));
            soundEffects.Add("Snow_Walk_3", Content.Load<SoundEffect>(@"Audio\snow_walk_3"));
            soundEffects.Add("Snow_Walk_4", Content.Load<SoundEffect>(@"Audio\snow_walk_4"));
            soundEffects.Add("Snow_Walk_5", Content.Load<SoundEffect>(@"Audio\snow_walk_5"));
            soundEffects.Add("Snow_Walk_6", Content.Load<SoundEffect>(@"Audio\snow_walk_6"));
            soundEffects.Add("Snow_Walk_7", Content.Load<SoundEffect>(@"Audio\snow_walk_7"));
            soundEffects.Add("Snow_Walk_8", Content.Load<SoundEffect>(@"Audio\snow_walk_8"));

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
            if (day > 30)
            {
                // Win condition
                deathText = "You Win!";
                state = State.GameState.Death_Load;
            }
            
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
                        ref _spriteBatch,
                        fonts,
                        textures
                        );
                    state = State.GameState.Intro_Main;
                    break;

                case State.GameState.Intro_Main:
                    introScreen.RunLogic(
                        ref state,
                        mouseButtonsHeld,
                        mouseState,
                        scale
                        );
                    break;

                case State.GameState.Day_Load:
                    day++;
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
                        keyMap,
                        soundEffects,
                        textures,
                        ref totalCoal,
                        gameSpeed,
                        gameTime.ElapsedGameTime.TotalSeconds,
                        keyboardState.GetPressedKeys()
                        );
                    break;

                case State.GameState.DayEnd_Load:
                    dayEndScreen = new DayEnd_Screen(
                        ref _graphics,
                        ref _spriteBatch,
                        fonts,
                        textures,
                        ref totalCoal,
                        coalQuota,
                        heatingCost
                        );
                    state = State.GameState.DayEnd_Main;
                    break;

                case State.GameState.DayEnd_Main:
                    dayEndScreen.RunLogic(
                        ref state,
                        mouseButtonsHeld,
                        mouseState,
                        scale
                        );
                    break;
                case State.GameState.Death_Load:
                    deathScreen = new Death_Screen(
                        ref _graphics,
                        ref _spriteBatch,
                        fonts,
                        textures
                        );
                    state = State.GameState.Death_Main;
                    break;
                case State.GameState.Death_Main:
                    deathScreen.RunLogic(
                        ref state,
                        mouseButtonsHeld,
                        mouseState,
                        scale
                        );
                    break;

            }

            // Set keys as held
            foreach(KeyValuePair<Keys, bool> kvp in keyMap)
            {
                // Set the key pressed as held
                if (keyboardState.IsKeyDown(kvp.Key))
                {
                    keyMap[kvp.Key] = true;
                }
                // Set the key as released
                else
                {
                    keyMap[kvp.Key] = false;
                }
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
                    GraphicsDevice.Clear(new Color(40, 40, 40, 255));
                    introScreen.RunGraphics(
                        _spriteBatch,
                        fonts,
                        scale
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
                    GraphicsDevice.Clear(new Color(40, 40, 40, 255));
                    dayEndScreen.RunGraphics(
                        _spriteBatch,
                        fonts,
                        scale,
                        coalQuota,
                        heatingCost,
                        totalCoal,
                        day
                        );
                    break;
                case State.GameState.Death_Main:
                    GraphicsDevice.Clear(new Color(40, 40, 40, 255));
                    deathScreen.RunGraphics(
                        _spriteBatch,
                        fonts,
                        scale,
                        deathText
                        );
                    break;
            }

            /// End draw
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
