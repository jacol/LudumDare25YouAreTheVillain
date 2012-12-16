using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace You_are_the_Villain.GameObjects
{
    internal class Village : GameObjectBase
    {        
        private SpriteBatch _spriteBatch;
        private Texture2D _spriteTexture;

        public Vector2 StartPosition { get; private set; }
        public int LabyrinthCorX { get; private set; }
        public int LabyrinthCorY { get; private set; }

        internal Village(Game game, Vector2 startPosition, int labyrinthCorX, int labyrinthCorY)
            :base(game, startPosition)
        {
            startPosition.Y += 5;
            startPosition.X += 5;
            StartPosition = startPosition;

            LabyrinthCorX = labyrinthCorX;
            LabyrinthCorY = labyrinthCorY;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteTexture = Game.Content.Load<Texture2D>("Home");

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_spriteTexture, StartPosition, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        internal void ChangeLocation(Vector2 newLocation, int newX, int newY)
        {
            LabyrinthCorX = newX;
            LabyrinthCorY = newY;

            StartPosition = newLocation;
        }
    }
}
