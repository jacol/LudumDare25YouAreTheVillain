using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.Objects.Labyrinth;
using Microsoft.Xna.Framework;

namespace GameLibrary.Algorithms.Labyrinth
{
    public class LabyrinthPathFinder
    {        
        private readonly Objects.Labyrinth.Labyrinth _labyrinth;        
        private int[,] _tempLabirynth;        

        public LabyrinthPathFinder(GameLibrary.Objects.Labyrinth.Labyrinth labyrinth)
        {
            _labyrinth = labyrinth;           
        }

        public Path FindWay(Game game, Coordinate from, Coordinate to)
        {
            List<string> visitedCoordinates = new List<string>();
             var currentCoordinate = new Coordinate(from);

            _tempLabirynth = new int[_labyrinth.Width, _labyrinth.Height];
            
            int step = 1;

            _tempLabirynth[currentCoordinate.X, currentCoordinate.Y] = step++;
            visitedCoordinates.Add(currentCoordinate.X.ToString() + "." + currentCoordinate.Y.ToString());

            Coordinate nextCoordinate;
            Move(currentCoordinate, to, step);

            while((nextCoordinate = FindNextCoordinate(ref step, visitedCoordinates)) != null)
            {
                Move(nextCoordinate, to, step + 1);
                visitedCoordinates.Add(nextCoordinate.X.ToString() + "." + nextCoordinate.Y.ToString());
            }

            var path = new Path(game, Color.Red) { Target = to };
            var coordinatesFromLastOne = new List<Coordinate>();
            if(PathFound(_tempLabirynth, to))
            {                                
                coordinatesFromLastOne.Add(new Coordinate(to.X, to.Y));
                int lastStep = _tempLabirynth[to.X, to.Y];

                for(int i = lastStep - 1;i>=1;i--)
                {
                    coordinatesFromLastOne.Add(GetCoordinate(_tempLabirynth, i, coordinatesFromLastOne.Last()));
                }
            }
            else
            {
                int firstX, firstY;
                int highestValue = FindHighest(_tempLabirynth, out firstX, out firstY);
                coordinatesFromLastOne.Add(new Coordinate(firstX, firstY));

                for (int i = highestValue - 1; i >= 1; i--)
                {
                    coordinatesFromLastOne.Add(GetCoordinate(_tempLabirynth, i, coordinatesFromLastOne.Last()));
                }
            }

            coordinatesFromLastOne.Reverse();
            foreach (var coordinate in coordinatesFromLastOne)
            {
                path.AddCoordinate(coordinate);
            }

            if (path.CoordinatesCollection.Last().X == to.X && path.CoordinatesCollection.Last().Y == to.Y) path.DestinationReached = true;
            return path;
        }

        private Coordinate FindNextCoordinate(ref int step, List<string> visitedCoordinates)
        {
            Coordinate coordinateForNextStep = null;

            for (int i = 0; i < _labyrinth.Height; i++)
            {
                for (int k = 0; k < _labyrinth.Width; k++)
                {
                    string current = k.ToString() + "." + i.ToString();
                    if (visitedCoordinates.Contains(current)) continue;

                    if(_tempLabirynth[k, i] == step)
                    {
                        return new Coordinate(k, i);
                    }

                    if (_tempLabirynth[k, i] == step + 1)
                    {
                        coordinateForNextStep = new Coordinate(k, i);
                    }
                }
            }

            step++;
            return coordinateForNextStep;
        }

        private int FindHighest(int[,] _tempLabirynth, out int x, out int y)
        {
            int max = 0;
            x = 0;
            y = 0;

            for (int i = 0; i < _labyrinth.Height; i++)
            {
                for (int k = 0; k < _labyrinth.Width; k++)
                {
                    if (_tempLabirynth[k, i] > max)
                    {
                        max = _tempLabirynth[k, i];
                        x = k;
                        y = i;
                    }
                }
            }

            return max;
        }

        private Coordinate GetCoordinate(int[,] _tempLabirynth, int searchValue, Coordinate startCoordinate)
        {
            //try right
            if (IsCellInsideLabyrinth(startCoordinate.X + 1, startCoordinate.Y) && CanMoveTo(startCoordinate, MoveDirection.Right) && startCoordinate.X + 1 < _labyrinth.Width && _tempLabirynth[startCoordinate.X + 1, startCoordinate.Y] == searchValue)
            {
                return new Coordinate(startCoordinate.X + 1, startCoordinate.Y);
            }

            //try left
            if (IsCellInsideLabyrinth(startCoordinate.X - 1, startCoordinate.Y) && CanMoveTo(startCoordinate, MoveDirection.Left) && startCoordinate.X - 1 >= 0 && _tempLabirynth[startCoordinate.X - 1, startCoordinate.Y] == searchValue)
            {
                return new Coordinate(startCoordinate.X - 1, startCoordinate.Y);
            }

            //try up
            if (IsCellInsideLabyrinth(startCoordinate.X, startCoordinate.Y - 1) && CanMoveTo(startCoordinate, MoveDirection.Up) && startCoordinate.Y - 1 >= 0 && _tempLabirynth[startCoordinate.X, startCoordinate.Y - 1] == searchValue)
            {
                return new Coordinate(startCoordinate.X, startCoordinate.Y - 1);
            }

            //try down
            if (IsCellInsideLabyrinth(startCoordinate.X, startCoordinate.Y + 1) && CanMoveTo(startCoordinate, MoveDirection.Down) && startCoordinate.Y + 1 < _labyrinth.Height && _tempLabirynth[startCoordinate.X, startCoordinate.Y + 1] == searchValue)
            {
                return new Coordinate(startCoordinate.X, startCoordinate.Y + 1);
            }

            throw new ApplicationException(string.Format("Cannot find {0} in tempLabyrinth", searchValue));
        }

        private bool PathFound(int[,] _tempLabirynth, Coordinate to)
        {
            return _tempLabirynth[to.X, to.Y] != 0;
        }

        private void Move(Coordinate currentCoordinate, Coordinate target, int step)
        {                        
            //reached target
            if (currentCoordinate.Equals(target))
            {
                //_pathFound = true;
                return;
            }

            //try up
            if(IsCellInsideLabyrinth(currentCoordinate.X, currentCoordinate.Y - 1) && CanMoveTo(currentCoordinate, MoveDirection.Up) && !HasBeenThere(new Coordinate(currentCoordinate.X, currentCoordinate.Y - 1)))
            {
                _tempLabirynth[currentCoordinate.X, currentCoordinate.Y - 1] = step;
                //Move(new Coordinate(currentCoordinate.X, currentCoordinate.Y - 1), target, step + 1);
            }

            //try down
            if (IsCellInsideLabyrinth(currentCoordinate.X, currentCoordinate.Y + 1) && CanMoveTo(currentCoordinate, MoveDirection.Down) && !HasBeenThere(new Coordinate(currentCoordinate.X, currentCoordinate.Y + 1)))
            {
                _tempLabirynth[currentCoordinate.X, currentCoordinate.Y + 1] = step;
                //Move(new Coordinate(currentCoordinate.X, currentCoordinate.Y + 1), target, step + 1);
            }
            
            //try left
            if(IsCellInsideLabyrinth(currentCoordinate.X - 1, currentCoordinate.Y) && CanMoveTo(currentCoordinate, MoveDirection.Left) && !HasBeenThere(new Coordinate(currentCoordinate.X - 1, currentCoordinate.Y)))
            {
                _tempLabirynth[currentCoordinate.X - 1, currentCoordinate.Y] = step;
                //Move(new Coordinate(currentCoordinate.X - 1, currentCoordinate.Y), target, step + 1);
            }

            //try right
            if (IsCellInsideLabyrinth(currentCoordinate.X + 1, currentCoordinate.Y) && CanMoveTo(currentCoordinate, MoveDirection.Right) && !HasBeenThere(new Coordinate(currentCoordinate.X + 1, currentCoordinate.Y)))
            {
                _tempLabirynth[currentCoordinate.X + 1, currentCoordinate.Y] = step;
                //Move(new Coordinate(currentCoordinate.X + 1, currentCoordinate.Y), target, step + 1);                
            }
        }

        private bool IsCellInsideLabyrinth(int x, int y)
        {
            return x >= 0 && y >= 0 && x < _labyrinth.Width && y < _labyrinth.Height;
        }

        private bool HasBeenThere(Coordinate coordinate)
        {
            return _tempLabirynth[coordinate.X, coordinate.Y] != 0;
        }

        private bool CanMoveTo(Coordinate currentCoordinate, MoveDirection moveDirection)
        {
            switch (moveDirection)
            {
                case MoveDirection.Up:
                    return _labyrinth[currentCoordinate.X, currentCoordinate.Y].Wall != WallType.Top && _labyrinth[currentCoordinate.X, currentCoordinate.Y - 1].Wall != WallType.Bottom;                    
                case MoveDirection.Down:
                    return _labyrinth[currentCoordinate.X, currentCoordinate.Y].Wall != WallType.Bottom && _labyrinth[currentCoordinate.X, currentCoordinate.Y + 1].Wall != WallType.Top;
                case MoveDirection.Left:
                    return _labyrinth[currentCoordinate.X, currentCoordinate.Y].Wall != WallType.Left && _labyrinth[currentCoordinate.X - 1, currentCoordinate.Y].Wall != WallType.Right;
                case MoveDirection.Right:
                    return _labyrinth[currentCoordinate.X, currentCoordinate.Y].Wall != WallType.Right && _labyrinth[currentCoordinate.X + 1, currentCoordinate.Y].Wall != WallType.Left;
                default:
                    throw new ArgumentOutOfRangeException("moveDirection");
            }
        }
    }
}
