using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace You_are_the_Villain.GameObjects
{
    internal class GameObjectBase : DrawableGameComponent
    {
        public virtual Rectangle CollisionRectangle
        {
            get;
            set;
        }

        internal GameObjectBase(Game game, Vector2 startPosition)
            :base(game)
        {
            CollisionRectangle = new Rectangle((int)startPosition.X, (int)startPosition.Y, 40, 40);
        }
    }
}
