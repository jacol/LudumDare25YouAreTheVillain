using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameLibrary.Objects.Labyrinth
{
    public class Labyrinth : DrawableGameComponent
    {
        private readonly LabyrinthUnit[,] _data;
        private readonly Game _game;

        public int Width { get { return _data.GetUpperBound(0); } }
        public int Height { get { return _data.GetUpperBound(1); } }

        public Labyrinth(Game game, LabyrinthUnit [,] data)
            :base(game)
        {
            _data = data;
            _game = game;
        }

        public LabyrinthUnit this[long x, long y]
        {
            get
            {                
                if(y < Height && x < Width)
                {
                    return _data[x, y];
                }
                throw new ApplicationException(string.Format("Cannot access labyrinth unit at ({0},{1})", x, y));
            }
        }

        public override void Initialize()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int k = 0; k < Height; k++)
                {
                    _game.Components.Add(_data[i, k]);
                }
            }

            base.Initialize();
        }

        public void RandomizeWalls()
        {
            Random random = new Random((int)DateTime.Now.Ticks);

            for (int i = 0; i < Width; i++)
            {
                for (int k = 0; k < Height; k++)
                {
                    if(random.Next() % 3 == 0)          //33% percent of walls will change
                    {
                        WallType newWallType = (WallType)(random.Next() % 4);
                        _data[i, k].ChangeWallType(newWallType);
                    } 
                }
            }
        }

        #region ToString method

        public override string ToString()
        {
            StringBuilder labString = new StringBuilder();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    labString.Append(_data[x, y].ToString());
                }

                labString.Append(Environment.NewLine);
            }

            return labString.ToString();
        }

        #endregion

        public void Remove(int i, int k)
        {
            _data[i, k].Deleted = true;
            _data[i, k].Wall = WallType.None;
            //_data[i, k].Color = Color.Red;
        }

        public void Build(int i, int k)
        {
            Random rand = new Random(DateTime.Now.Millisecond);

            _data[i, k].Deleted = false;
            
            WallType wallType = _data[i,k].Wall;

            if(wallType == WallType.None) _data[i, k].ChangeWallType((WallType)rand.Next(3));
            else if (wallType == WallType.Right) _data[i, k].ChangeWallType(WallType.Bottom);
            else if (wallType == WallType.Bottom) _data[i, k].ChangeWallType(WallType.Left);
            else if (wallType == WallType.Left) _data[i, k].ChangeWallType(WallType.Top);
            else if (wallType == WallType.Top) _data[i, k].ChangeWallType(WallType.Right);
            _data[i, k].SetCurrentPosition();
        }
    }
}
