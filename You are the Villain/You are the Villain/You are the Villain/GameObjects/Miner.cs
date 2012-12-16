using GameLibrary.Algorithms.Labyrinth;
using GameLibrary.Objects.Labyrinth;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace You_are_the_Villain.GameObjects
{
    internal class Miner : GameObjectBase
    {
        private int _gatheredGold = 0, _stepsToComplete = 0;

        private MinerState _minerState = MinerState.GoingToMine;
        private bool _haveGold = false;

        private TimeSpan _thinkTime = new TimeSpan(0, 0, 0, 0, 500);
        private TimeSpan _thinkStart;

        private TimeSpan _moveTime = new TimeSpan(0, 0, 0, 0, 5);
        private TimeSpan _lastMove = DateTime.Now.TimeOfDay;

        private TimeSpan _timeOffset, _lastTimeOffsetCheck;        

        private SpriteBatch _spriteBatch;
        private Texture2D _spriteTexture, _spriteTextureThink, _spriteTextureWithGold;

        private Vector2 _currentPosition, _currentDestination;
        public int LabyrinthCorX { get; private set; }
        public int LabyrinthCorY { get; private set; }

        private static int _villageCorX, _villageCorY;

        private static int _mineCorX, _mineCorY;

        public override Rectangle CollisionRectangle
        {
            get
            {
                return new Rectangle((int)_currentPosition.X, (int)_currentPosition.Y, 40, 40);
            }
            set
            {
                base.CollisionRectangle = value;
            }
        }

        internal Miner(Game game, Vector2 startPosition, int labyrinthCorX, int labyrinthCorY, int mineCorX, int mineCorY, TimeSpan timeOffset)
            : base(game, startPosition)
        {
            //startPosition.Y += 25;
            //startPosition.X += 25;
            _currentPosition = startPosition;
            _currentDestination = _currentPosition;

            LabyrinthCorX = labyrinthCorX;
            LabyrinthCorY = labyrinthCorY;

            _villageCorX = LabyrinthCorX;
            _villageCorY = labyrinthCorY;

            _mineCorX = mineCorX;
            _mineCorY = mineCorY;

            _timeOffset = timeOffset;
            _lastTimeOffsetCheck = DateTime.Now.TimeOfDay;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteTexture = Game.Content.Load<Texture2D>("aMiner");
            _spriteTextureThink = Game.Content.Load<Texture2D>("aMiner1");
            _spriteTextureWithGold = Game.Content.Load<Texture2D>("MinerGold");

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            if (DateTime.Now.Subtract(_lastTimeOffsetCheck).TimeOfDay > _timeOffset)
            {
                _spriteBatch.Begin();
                if (_minerState == MinerState.Thinking)
                {
                    _spriteBatch.Draw(_spriteTextureThink, _currentPosition, Color.White);
                }
                else if (_minerState == MinerState.GoingToMine)
                {
                    _spriteBatch.Draw(_spriteTexture, _currentPosition, Color.White);
                }
                else if (_minerState == MinerState.GoingToVillage)
                {
                    _spriteBatch.Draw(_spriteTextureWithGold, _currentPosition, Color.White);
                }

                _spriteBatch.End();

                base.Draw(gameTime);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (DateTime.Now.Subtract(_lastTimeOffsetCheck).TimeOfDay > _timeOffset)
            {
                if (DateTime.Now.Subtract(_thinkStart).TimeOfDay <= _thinkTime) return; //still thinking

                if (ReachedDestination())
                {
                    Vector2 minePosition = CalculateLabCoordinatesToVector(_mineCorX, _mineCorY);
                    Vector2 villagePosition = CalculateLabCoordinatesToVector(_villageCorX, _villageCorY);//those 2 can be moved to init and calc once!

                    if(_currentPosition == villagePosition && _haveGold)
                    {
                        CollectGOld();
                    }
                    else if (_currentPosition == minePosition || _haveGold)
                    {
                        _haveGold = true;                        
                        FindNextDestination(false);
                    }                    
                    else
                    {
                        FindNextDestination(true);
                    }
                }
                else
                {
                    GotoNextDestination();
                }
                
                base.Update(gameTime);
            }
        }

        private void CollectGOld()
        {
            _haveGold = false;
            _gatheredGold++;
            Game.Content.Load<SoundEffect>("GoldCollected").Play();
            
            Score score = (Score)Game.Services.GetService(typeof(Score));
            score.DecreaseScore(200 / _stepsToComplete);
            _stepsToComplete = 0;
        }

        private void GotoNextDestination()
        {
            _minerState = _haveGold ? MinerState.GoingToVillage : MinerState.GoingToMine;

            if (DateTime.Now.Subtract(_lastMove).TimeOfDay > _moveTime)
            {
                if (_currentPosition.X > _currentDestination.X) _currentPosition.X--;
                else if (_currentPosition.X < _currentDestination.X) _currentPosition.X++;

                if (_currentPosition.Y > _currentDestination.Y) _currentPosition.Y--;
                else if (_currentPosition.Y < _currentDestination.Y) _currentPosition.Y++;

                _lastMove = DateTime.Now.TimeOfDay;
            }
        }

        private bool ReachedDestination()
        {
            return _currentPosition.X == _currentDestination.X && _currentPosition.Y == _currentDestination.Y;
        }

        private void FindNextDestination(bool goingToMine)
        {
            _minerState = MinerState.Thinking;
            _thinkStart = DateTime.Now.TimeOfDay;
            _stepsToComplete++;

            Path path = null;
            LabyrinthPathFinder labyrinthPathFinder = (LabyrinthPathFinder)Game.Services.GetService(typeof(LabyrinthPathFinder));
            if (goingToMine)
            {                
                path = labyrinthPathFinder.FindWay(base.Game, new Coordinate(LabyrinthCorX, LabyrinthCorY), new Coordinate(_mineCorX, _mineCorY));
            }
            else
            {
                path = labyrinthPathFinder.FindWay(base.Game, new Coordinate(LabyrinthCorX, LabyrinthCorY), new Coordinate(_villageCorX, _villageCorY));
            }

            if (path.DestinationReached == false)
            {
                ChangeVillageLocation();
                ChangeMineLocation();
            }

            if (path.CoordinatesCollection.Count > 1) path.CoordinatesCollection.RemoveAt(0);
            Random rand = new Random(DateTime.Now.Millisecond);
            _thinkTime = new TimeSpan(0, 0, 0, 0, path.CoordinatesCollection.Count * rand.Next(50));
            //Game.Components.Add(path);            

            Vector2 newPosition = GetFirstPathStepConverted(path);

            _currentDestination = newPosition;            

            LabyrinthCorX = ((int)newPosition.X - 25) / LabyrinthUnit.Length;
            LabyrinthCorY = ((int)newPosition.Y - 25) / LabyrinthUnit.Length;
        }

        private void ChangeMineLocation()
        {
            Mine mine = (Mine)Game.Services.GetService(typeof(Mine));
            Labyrinth labyrinth = (Labyrinth)Game.Services.GetService(typeof(Labyrinth));

            Random rand = new Random(DateTime.Now.Millisecond);

            int x = rand.Next(labyrinth.Width);            
            int y = rand.Next(labyrinth.Height);

            while (x == _villageCorX) x = rand.Next(labyrinth.Width);
            while (y == _villageCorY) y = rand.Next(labyrinth.Height);   

            _mineCorX = x;
            _mineCorY = y;

            mine.ChangeLocation(CalculateLabCoordinatesToVector(x, y), x, y);
        }

        private void ChangeVillageLocation()
        {
            Village village = (Village)Game.Services.GetService(typeof(Village));
            Labyrinth labyrinth = (Labyrinth)Game.Services.GetService(typeof(Labyrinth));

            Random rand = new Random(DateTime.Now.Millisecond);

            int x = rand.Next(labyrinth.Width);
            int y = rand.Next(labyrinth.Height);

            while (x == _mineCorX) x = rand.Next(labyrinth.Width);
            while (y == _mineCorY) y = rand.Next(labyrinth.Height);   

            _villageCorX = x;
            _villageCorY = y;

            village.ChangeLocation(CalculateLabCoordinatesToVector(x, y), x, y);
        }

        private Vector2 GetFirstPathStepConverted(Path path)
        {            
            Coordinate newCoordinate = path.CoordinatesCollection.First();
            return CalculateLabCoordinatesToVector(newCoordinate.X, newCoordinate.Y);
            //int newX = (newCoordinate.X * (LabyrinthUnit.Length)) + 25;
            //int newY = (newCoordinate.Y * (LabyrinthUnit.Length)) + 25;

            //return new Vector2(newX, newY);
        }

        private Vector2 CalculateLabCoordinatesToVector(int corX, int corY)
        {
            return new Vector2(corX * (LabyrinthUnit.Length) + 25, corY * (LabyrinthUnit.Length) + 25);
        }
    }

    enum MinerState
    {
        GoingToMine,
        Thinking,
        GoingToVillage
    }
}
