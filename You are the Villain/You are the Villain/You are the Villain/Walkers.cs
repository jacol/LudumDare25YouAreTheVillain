using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using You_are_the_Villain.Managers;
using GameLibrary.Objects.Labyrinth;
using You_are_the_Villain.GameObjects;
using GameLibrary.Algorithms.Labyrinth;
using You_are_the_Villain.CollisionVeryfier;

namespace You_are_the_Villain
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Walkers : Microsoft.Xna.Framework.Game
    {
        public static bool WereKilled = false;

        public static TimeSpan GameTime = new TimeSpan(0, 3, 0);
        private DateTime _startTime;

        private static Color Color = Color.Orange;
        private SpriteFont PositionFont;        

        GameState _gameState = GameState.StartScreen;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        StartScreen _startScreen;

        public Walkers()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 700;
            graphics.PreferredBackBufferWidth = 1000;
            //graphics.ToggleFullScreen();
            
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            _startScreen = new StartScreen(this);
            
            this.Components.Add(_startScreen);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            PositionFont = Content.Load<SpriteFont>("ScoreSpriteFont");

            Song song = Content.Load<Song>("music");
            MediaPlayer.Volume = 0.5f;
            MediaPlayer.Play(song);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            if (Keyboard.GetState(PlayerIndex.One).GetPressedKeys().Contains(Keys.Escape))
                this.Exit();

            if (Keyboard.GetState(PlayerIndex.One).GetPressedKeys().Contains(Keys.Enter) && _gameState == GameState.StartScreen)
            {
                this.Components.Remove(_startScreen);
                //start gamr
                GameObjectsManager gameObjectsManager = new GameObjectsManager(this, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
                gameObjectsManager.Initialize();
                _gameState = GameState.InProgress;
                _startTime = DateTime.Now;
            }

            if (WereKilled || (_gameState == GameState.InProgress && GameTime - DateTime.Now.Subtract(_startTime) <= new TimeSpan(0, 0, 0)))
            {
                this.Components.Clear();
                _gameState = GameState.Finish;
            }            

            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            if (_gameState == GameState.InProgress)
            {
                string timeText = string.Format("Time left: {0}", GameTime - DateTime.Now.Subtract(_startTime));

                Vector2 FontOrigin = PositionFont.MeasureString(timeText) / 2;
                spriteBatch.DrawString(
                    PositionFont,
                    timeText,
                    new Vector2(160, 655),
                    Color,
                    0,
                    FontOrigin,
                    1.0f,
                    SpriteEffects.None,
                    0.5f);                
            }
            else if (_gameState == GameState.Finish)
            {
                Score score = (Score)Services.GetService(typeof(Score));
                string timeText =
                    "Game was created during 25. Ludum Dare contest: http://www.ludumdare.com" +
                    Environment.NewLine +
                    "December 14th-17th, 2012" +
                    Environment.NewLine +
                    "Thanks for playing!" +
                    Environment.NewLine + Environment.NewLine +
                    (WereKilled ? "You were killed!!! :)" : string.Empty) +
                    Environment.NewLine + Environment.NewLine +
                    string.Format("Your score: {0}", score.ScoreValue) +
                    Environment.NewLine + Environment.NewLine +
                    "Press <Escape> to exit.";

                Vector2 FontOrigin = PositionFont.MeasureString(timeText) / 2;
                spriteBatch.DrawString(
                    PositionFont,
                    timeText,
                    new Vector2(500, 255),
                    Color,
                    0,
                    FontOrigin,
                    1.0f,
                    SpriteEffects.None,
                    0.5f); 
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }

    enum GameState
    {
        StartScreen,
        InProgress,
        Finish
    }
}
