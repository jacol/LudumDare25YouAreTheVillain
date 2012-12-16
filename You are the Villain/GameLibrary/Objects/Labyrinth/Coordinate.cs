using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLibrary.Objects.Labyrinth
{
    public class Coordinate
    {
        private readonly Tuple<int, int> _data;

        public int X
        {
            get { return _data.Item1; }
        }

        public int Y
        {
            get { return _data.Item2; }
        }

        public Coordinate(int x, int y)
        {
            _data = new Tuple<int, int>(x, y);
        }

        public Coordinate(Coordinate baseCoordinate)
        {
            _data =  new Tuple<int, int>(baseCoordinate.X, baseCoordinate.Y);
        }

        public override int GetHashCode()
        {
            return (X + Y).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Coordinate other = obj as Coordinate;
            if(other != null)
            {
                return other.X == X && other.Y == Y;
            }

            return false;
        }

        public override string ToString()
        {
            return string.Format("({0},{1})", X, Y);
        }
    }
}
