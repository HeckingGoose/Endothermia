﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

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
        private Dictionary<string, SpriteFont> fonts;

        // Textures
        private Dictionary<string, Texture2D> textures;

        // Window scale tracking
        private Vector2 scale;

        // Screens
        private uint state;
        private Menu_Screen menuScreen;
        private Intro_Screen introScreen;
        private Game_Screen gameScreen;

        // Mouse held
        private bool[] mouseButtonsHeld;

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
            fonts = new Dictionary<string, SpriteFont>();
            textures = new Dictionary<string, Texture2D>();
            mouseButtonsHeld = new bool[3];

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

            textures.Add("ButtonTexture", Content.Load<Texture2D>(@"Sprites\ButtonTexture"));
            textures.Add("TitleTexture", Content.Load<Texture2D>(@"Sprites\blackTitle"));
            textures.Add("Coal", Content.Load<Texture2D>(@"Sprites\coal"));
            textures.Add("Oil", Content.Load<Texture2D>(@"Sprites\oil"));
            textures.Add("Gas", Content.Load<Texture2D>(@"Sprites\gas"));
            textures.Add("Rock", Content.Load<Texture2D>(@"Sprites\rock"));


            // Initialise menu screen
            menuScreen = new Menu_Screen(
                ref _graphics,
                ref _spriteBatch,
                textures,
                fonts
                );

            // Initialise intro screen
            introScreen = new Intro_Screen(
                ref _graphics,
                ref _spriteBatch
                );

            // Initialise game screen
            gameScreen = new Game_Screen(
                ref _graphics,
                ref _spriteBatch,
                19,
                25,
                0.05f,
                0.025f,
                0.015f,
                0.2f
                );
        }

        protected override void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();

            // Patch
            (int mouseX, int mouseY) mousePosition = (
                mouseState.Position.X - Window.Position.X,
                mouseState.Position.Y - Window.Position.Y
                );

            float gameSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * TARGET_FRAMERATE;

            switch (state)
            {
                case 0: // Menu screen
                    menuScreen.RunLogic(
                        ref state,
                        mouseButtonsHeld,
                        mouseState,
                        mousePosition
                        );
                    break;
                case 1: // Intro screen
                    introScreen.RunLogic(
                        );
                    break;
                case 2: // Main game loop
                    gameScreen.RunLogic(
                        gameSpeed
                        );
                    break;
            }
            
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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Begin draw
            _spriteBatch.Begin();

            switch (state)
            {
                case 0: // Menu screen
                    menuScreen.RunGraphics(
                        _spriteBatch,
                        textures
                        );
                    break;
                case 1: // Intro screen
                    introScreen.RunGraphics(
                        _spriteBatch
                        );
                    break;
                case 2: // Main game loop
                    gameScreen.RunGraphics(
                        _spriteBatch,
                        textures
                        );
                    break;
            }

            /// End draw
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
