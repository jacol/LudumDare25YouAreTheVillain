using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLibrary.Objects.Labyrinth
{
    public class Path : DrawableGameComponent
    {
        private SpriteFont PositionFont;        
        private SpriteBatch _spriteBatch;

        private readonly List<Coordinate> _coordinatesCollection;
        private readonly Game _game;
        private Color _color;

        public Color Color { get { return _color; } }

        public bool ReachedTarget { get; set; }
        public Coordinate Target { get; set; }

        public Game Game { get { return _game; } }

        public bool DestinationReached { get; set; }
        public Coordinate From { get { return _coordinatesCollection.First(); } }
        public Coordinate To { get { return _coordinatesCollection.Last(); } }
        public List<Coordinate> CoordinatesCollection { get { return _coordinatesCollection; } } 

        public Path(Game game, Color color)
            :base(game)
        {
            _game = game;
            _color = color;
            _coordinatesCollection = new List<Coordinate>();
        }

        public Path(Path copyFrom, Color color)
            : base(copyFrom.Game)
        {
            _game = copyFrom.Game;
            _color = color;
            _coordinatesCollection = new List<Coordinate>(copyFrom.CoordinatesCollection);
            DestinationReached = copyFrom.DestinationReached;
        }

        public void AddCoordinate(Coordinate step)
        {
            if(_coordinatesCollection.Contains(step))
            {
                throw new ApplicationException("Steps on path cannot be duplicated");
            }

            _coordinatesCollection.Add(step);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            PositionFont = Game.Content.Load<SpriteFont>("SpriteFont");

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            int counter = 0;
            foreach (var coordinate in CoordinatesCollection)
            {
                //string positionText = string.Format("{0} ({1}, {2})", counter++, coordinate.X, coordinate.Y);
                string positionText = string.Format("{0}", counter++);

                Vector2 FontOrigin = PositionFont.MeasureString(positionText)/2;
                _spriteBatch.DrawString(
                    PositionFont, 
                    positionText, 
                    new Vector2(coordinate.X * LabyrinthUnit.Length + 24, coordinate.Y * LabyrinthUnit.Length + 24), 
                    _color, 
                    0, 
                    FontOrigin,
                    1.0f, 
                    SpriteEffects.None,
                    0.5f);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }


        public override bool Equals(object obj)
        {
            if (obj is Path)
            {
                Path other = obj as Path;
                if (other.CoordinatesCollection.Count == this.CoordinatesCollection.Count)
                {
                    int count = 0;
                    foreach (var coordinate in CoordinatesCollection)
                    {
                        if (other.CoordinatesCollection[count].X != coordinate.X || other.CoordinatesCollection[count++].Y != coordinate.Y)
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }

            return false;            
        }

        public override int GetHashCode()
        {
            return string.Join(".", _coordinatesCollection.OrderBy(c => c.X).ThenBy(c => c.Y).Select(c => c.X.ToString() + c.Y.ToString())).GetHashCode();
        }
    }
}
