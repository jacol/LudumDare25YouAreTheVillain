using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace You_are_the_Villain
{
    class Border : DrawableGameComponent
    {
        public int BorderWidth, BorderHeight;
        
        public const int BorderThick = 5;

        private const int BorderTop = 20;
        private const int BorderLeft = 20;

        private static readonly Color Color = Microsoft.Xna.Framework.Color.DarkGray;

        private SpriteBatch _spriteBatch;
        private Texture2D _vspriteTexture, _hspriteTexture;

        public Vector2 TopVector { get; set; }
        public Vector2 BottomVector { get; set; }
        public Vector2 LeftVector { get; set; }
        public Vector2 RightVector { get; set; }

        public Rectangle[] CollisionRectangles
        {
            get
            {
                return new Rectangle[] 
                {
                    new Rectangle((int)TopVector.X, (int)BottomVector.Y, BorderWidth, BorderThick),
                    new Rectangle((int)BottomVector.X, (int)TopVector.Y, BorderWidth, BorderThick),
                    new Rectangle((int)RightVector.X, (int)TopVector.Y, BorderThick, BorderHeight),
                    new Rectangle((int)LeftVector.X, (int)TopVector.Y, BorderThick, BorderHeight)
                };
            }
        }

        internal Border(Game game, int screenWidth, int screenHeight)
            : base(game)
        {
            BorderHeight = screenHeight - BorderThick;
            BorderWidth = screenWidth - BorderThick;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Color[] vdata = new Color[BorderThick * BorderHeight];
            _vspriteTexture = new Texture2D(GraphicsDevice, BorderThick, BorderHeight);
            for (int i = 0; i < vdata.Length; ++i) vdata[i] = Color;
            _vspriteTexture.SetData(vdata);

            Color[] hdata = new Color[BorderThick * BorderWidth];
            _hspriteTexture = new Texture2D(GraphicsDevice, BorderWidth, BorderThick);
            for (int i = 0; i < hdata.Length; ++i) hdata[i] = Color;
            _hspriteTexture.SetData(hdata);

            LeftVector = new Vector2(BorderLeft, BorderTop);
            RightVector = new Vector2(BorderLeft + BorderWidth, BorderTop + BorderThick);
            TopVector = new Vector2(BorderLeft + BorderThick, BorderTop);
            BottomVector = new Vector2(BorderLeft, BorderTop + BorderHeight);

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            _spriteBatch.Draw(_vspriteTexture, LeftVector, Color);
            _spriteBatch.Draw(_vspriteTexture, RightVector, Color);
            _spriteBatch.Draw(_hspriteTexture, TopVector, Color);
            _spriteBatch.Draw(_hspriteTexture, BottomVector, Color);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
