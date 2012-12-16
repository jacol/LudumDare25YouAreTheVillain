using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace You_are_the_Villain
{
    class StartScreen : DrawableGameComponent
    {
        private static Color Color = Color.Orange;
        private Texture2D _spriteTexture;
        private SpriteFont PositionFont;
        private SpriteBatch _spriteBatch;
        

        internal StartScreen(Game game)
            : base(game)
        {                   
        }
        

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            PositionFont = Game.Content.Load<SpriteFont>("ScoreSpriteFont");
            _spriteTexture = Game.Content.Load<Texture2D>("logo");
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            _spriteBatch.Draw(_spriteTexture, new Vector2(350, 260), Color.White);

            string scoreText = "Disturb the miners!" +
                Environment.NewLine + Environment.NewLine +
                "The more steps miner need to make to bring gold, the less points you will be taken." +
                Environment.NewLine +
                "Miners can kill you!" +
                Environment.NewLine +
                "If you block any miner from mine or village - they will find new structures!" +
                Environment.NewLine +
                "Use arrow keys to control the monster."+
                Environment.NewLine +
                "Use 'X' to destroy marked walls and 'Z' to change order of marked walls." +
                Environment.NewLine +
                "Some walls were destroyed in order to allow miners enter the mine." + Environment.NewLine +
                "Feel free to find them and rebuild (using 'Z')." +
                Environment.NewLine + Environment.NewLine +
                "Use <Escape> to exit anytime." +
                Environment.NewLine + Environment.NewLine +
                "Press <ENTER> to start...";

            Vector2 FontOrigin = PositionFont.MeasureString(scoreText) / 2;
            _spriteBatch.DrawString(
                PositionFont,
                scoreText,
                new Vector2(500, 180),
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
