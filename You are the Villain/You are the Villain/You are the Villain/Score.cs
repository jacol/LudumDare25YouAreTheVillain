using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace You_are_the_Villain
{
    class Score : DrawableGameComponent
    {
        private static Color Color = Color.Orange;

        private SpriteFont PositionFont;
        private SpriteBatch _spriteBatch;
        
        private int _screenWidth, _screenHeight;

        public int ScoreValue { get; private set; }

        internal Score(Game game, int screenWidth, int screenHeight, int score)
            :base(game)
        {
            _screenHeight = screenHeight;
            _screenWidth = screenWidth;
            ScoreValue = score;
        }

        public void DecreaseScore(int by)
        {
            ScoreValue -= by;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            PositionFont = Game.Content.Load<SpriteFont>("ScoreSpriteFont");

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            
            string scoreText = string.Format("Your score: {0}", ScoreValue);

            Vector2 FontOrigin = PositionFont.MeasureString(scoreText) / 2;
            _spriteBatch.DrawString(
                PositionFont,
                scoreText,
                new Vector2(100, _screenHeight - 20),
                Color,
                0,
                FontOrigin,
                1.0f,
                SpriteEffects.None,
                0.5f);
            
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
