using GameLibrary.Objects.Labyrinth;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace You_are_the_Villain.CollisionVeryfier
{
    internal class WallCollisionVerifier
    {
        private Border _border;
        private Labyrinth _labyrinth;

        internal WallCollisionVerifier(Border border, Labyrinth labyrinth)
        {
            _border = border;
            _labyrinth = labyrinth;
        }

        internal bool CanMoveUp(Rectangle collisionRect)
        {
            collisionRect.Y--;
            return CheckBordersAndWalls(collisionRect);
        }
       
        internal bool CanMoveDown(Rectangle collisionRect)
        {
            collisionRect.Y++;
            return CheckBordersAndWalls(collisionRect);
        }

        internal bool CanMoveLeft(Rectangle collisionRect)
        {
            collisionRect.X--;
            return CheckBordersAndWalls(collisionRect);
        }

        internal bool CanMoveRight(Rectangle collisionRect)
        {
            collisionRect.X++;
            return CheckBordersAndWalls(collisionRect);
        }

        private bool CheckBordersAndWalls(Rectangle collisionRect)
        {
            if (_border.CollisionRectangles.Any(r => r.Intersects(collisionRect))) return false;

            for (int i = 0; i < _labyrinth.Width; i++)
            {
                for (int k = 0; k < _labyrinth.Height; k++)
                {
                    if (_labyrinth[i, k].CollisionRectangle.Intersects(collisionRect)) return false;
                }
            }

            return true;
        }
    }
}
