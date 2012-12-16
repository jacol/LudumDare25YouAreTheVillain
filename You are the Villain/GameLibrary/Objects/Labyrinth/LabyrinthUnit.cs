using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLibrary.Objects.Labyrinth
{
    public class LabyrinthUnit : DrawableGameComponent
    {
        private SpriteFont PositionFont;  

        public bool Deleted = false;

        public bool IsOnPath { get { return !string.IsNullOrWhiteSpace(Number); } }
        public string Number { get; set; }
        public WallType Wall;

        //public const int Width = 3, Length = 25;
        public const int Width = 5, Length = 50;

        public Color Color = Microsoft.Xna.Framework.Color.Gray;

        private SpriteBatch _spriteBatch;
        private Texture2D _spriteTexture;

        public Vector2 CurrentPosition { get; set; }
        private readonly Vector2 _topRightPosition, _bottomLeftPosition;
        public Vector2 TopLeftPosition { get; private set; }

        private int _indexX, _indexY;

        public Rectangle CollisionRectangle
        {
            get
            {
                if (Deleted)
                    return new Rectangle();
                else
                    return Wall == WallType.Left || Wall == WallType.Right ? new Rectangle((int)CurrentPosition.X, (int)CurrentPosition.Y, Width, Length) :
                                                                        new Rectangle((int)CurrentPosition.X, (int)CurrentPosition.Y, Length, Width);
            }
        }

        public Rectangle CollisionRectangleForBuild
        {
            get
            {
                if (Deleted)
                    return new Rectangle((int)CurrentPosition.X, (int)CurrentPosition.Y, Length, Length);
                else
                    return Wall == WallType.Left || Wall == WallType.Right ? new Rectangle((int)CurrentPosition.X, (int)CurrentPosition.Y, Width, Length) :
                                                                        new Rectangle((int)CurrentPosition.X, (int)CurrentPosition.Y, Length, Width);
            }
        }

        public LabyrinthUnit(Game game, int posX, int posY, int indexX, int indexY, WallType wallType)
            : base(game)
        {
            Wall = wallType;

            TopLeftPosition = new Vector2(posX, posY);
            _topRightPosition = new Vector2(posX + Length, posY);
            _bottomLeftPosition = new Vector2(posX, posY + Length);

            SetCurrentPosition();

            _indexX = indexX;
            _indexY = indexY;
        }

        public void SetCurrentPosition()
        {
            if (Wall == WallType.Right) CurrentPosition = _topRightPosition;
            else if (Wall == WallType.Left) CurrentPosition = TopLeftPosition;
            else if (Wall == WallType.Bottom) CurrentPosition = _bottomLeftPosition;
            else CurrentPosition = TopLeftPosition;
        }

        #region Overriden Drawable Methods

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            SetDesignBasedOnWallType();

            PositionFont = Game.Content.Load<SpriteFont>("SpriteFont");

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {            
            _spriteBatch.Begin();

            //string positionText = Wall.ToString().First().ToString() + _indexX + "." + _indexY;

            //Vector2 FontOrigin = PositionFont.MeasureString(positionText) / 2;
            //_spriteBatch.DrawString(
            //    PositionFont,
            //    positionText,
            //    TopLeftPosition + new Vector2(20, 20),
            //    Color.Red,
            //    0,
            //    FontOrigin,
            //    0.7f,
            //    SpriteEffects.None,
            //    0.5f);

            if (!Deleted)
            {
                _spriteBatch.Draw(_spriteTexture, CurrentPosition, Color);
            }

            _spriteBatch.End();
            
            base.Draw(gameTime);
        }

        #endregion

        #region Public Methods

        public void ChangeWallType(WallType wallType)
        {
            Wall = wallType;
            SetDesignBasedOnWallType();
        }

        public int GetWidth()
        {
            if (Wall == WallType.Right || Wall == WallType.Left) return Width;
            else return Length;
        }

        public int GetHeight()
        {
            if (Wall == WallType.Right || Wall == WallType.Left) return Length;
            else return Width;
        }

        #endregion

        #region Private Methods

        private void SetDesignBasedOnWallType()
        {
            Color[] data;
            if (Wall == WallType.Bottom || Wall == WallType.Top)
            {
                data = new Color[Width * Length];
                _spriteTexture = new Texture2D(GraphicsDevice, Length, Width);
            }
            else
            {
                data = new Color[Width * (Length)];
                _spriteTexture = new Texture2D(GraphicsDevice, Width, Length);
            }

            Random rand = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < data.Length; ++i) data[i] = GetRandomGray(rand);
            _spriteTexture.SetData(data);
        }

        private Microsoft.Xna.Framework.Color GetRandomGray(Random rand)
        {
            int next = rand.Next(255);
            return new Color(next, next, next);
        }

        #endregion

        #region ToString overriden method

        public override string ToString()
        {
            switch (Wall)
            {
                case WallType.Top:
                    return IsOnPath ? Number : "^";
                case WallType.Bottom:
                    return IsOnPath ? Number : "_";
                case WallType.Left:
                    return IsOnPath ? Number : "<";
                case WallType.Right:
                    return IsOnPath ? Number : ">";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}
