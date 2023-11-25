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
        private Texture2D buttonTexture;
        private Texture2D titleTexture;

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
            fonts = new Dictionary<string, SpriteFont>();

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

            buttonTexture = Content.Load<Texture2D>(@"Sprites\ButtonTexture");
            titleTexture = Content.Load<Texture2D>(@"Sprites\blackTitle");


            // Initialise menu screen
            menuScreen = new Menu(
                ref _graphics,
                ref _spriteBatch,
                buttonTexture,
                fonts
                );
        }

        protected override void Update(GameTime gameTime)
        {
            switch (state)
            {
                case 0: // Menu screen
                    menuScreen.RunLogic(
                        ref state
                        );
                    break;
                case 1: // Intro screen
                    break;
                case 2: // Main game loop
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
                        titleTexture
                        );
                    break;
                case 1: // Intro screen
                    break;
                case 2: // Main game loop
                    break;
            }

            /// End draw
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
