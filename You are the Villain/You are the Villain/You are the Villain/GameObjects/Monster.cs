using GameLibrary.Objects.Labyrinth;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using You_are_the_Villain.CollisionVeryfier;

namespace You_are_the_Villain.GameObjects
{
    class Monster : GameObjectBase
    {
        private TimeSpan HurtOffset = new TimeSpan(0, 0, 2);
        private TimeSpan _lastHurt;

        private int _life = 4;

        private SpriteFont PositionFont;  

        private Point _frameSize = new Point(40, 40);
        private Point _currentFrame = new Point(0, 0);
        private Point _sheetSize = new Point(2, 2);
        private static TimeSpan TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 120);
        private TimeSpan _lastAnimationTime = new TimeSpan(DateTime.Now.Ticks);

        private static TimeSpan BombReloadTime = new TimeSpan(0, 0, 5);
        private TimeSpan _lastBomb = new TimeSpan(DateTime.Now.Ticks);

        private static TimeSpan BuildReloadTime = new TimeSpan(0, 0, 1);
        private TimeSpan _lastBuild = new TimeSpan(DateTime.Now.Ticks);

        private const int MovementStep = 1;

        private SpriteBatch _spriteBatch;
        private Texture2D _spriteTexture;

        private Vector2 _currentPosition;

        private ProgressBar _bombProgressBar, _buildProgressBar, _lifeProgressBar;

        public int LabyrinthCorX { get; private set; }
        public int LabyrinthCorY { get; private set; }

        internal Monster(Game game, Vector2 startPosition, int labyrinthCorX, int labyrinthCorY)
            : base(game, startPosition)
        {
            startPosition.Y += 6;
            startPosition.X += 6;
            _currentPosition = startPosition;

            LabyrinthCorX = labyrinthCorX;
            LabyrinthCorY = labyrinthCorY;

            _bombProgressBar = new ProgressBar(base.Game, new Rectangle(875, 630, 100, 20), ProgressBar.Orientation.HORIZONTAL_LR) { borderThicknessOuter = 1, borderThicknessInner = 0, maximum = 100, value = 0, fillColor = Color.Tomato };
            _buildProgressBar = new ProgressBar(base.Game, new Rectangle(875, 655, 100, 20), ProgressBar.Orientation.HORIZONTAL_LR) { borderThicknessOuter = 1, borderThicknessInner = 0, maximum = 100, value = 0, fillColor = Color.Green };
            _lifeProgressBar = new ProgressBar(base.Game, new Rectangle(875, 680, 100, 20), ProgressBar.Orientation.HORIZONTAL_LR) { borderThicknessOuter = 1, borderThicknessInner = 0, maximum = 4, value = 4, fillColor = Color.Red };
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteTexture = Game.Content.Load<Texture2D>("Monster");
            PositionFont = Game.Content.Load<SpriteFont>("SpriteFont");

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            //draw reloadings            
            _lifeProgressBar.Draw(_spriteBatch);
            _buildProgressBar.Draw(_spriteBatch);
            _bombProgressBar.Draw(_spriteBatch);

            _spriteBatch.Draw(_spriteTexture, _currentPosition,
                new Rectangle(_currentFrame.X * _frameSize.X,
                    _currentFrame.Y * _frameSize.Y,
                    _frameSize.X,
                    _frameSize.Y),
                Color.White, 0, Vector2.Zero,
                1, SpriteEffects.None, 0);

            
            
            
            Vector2 FontOrigin = PositionFont.MeasureString("Blow up (x)") / 2;
            _spriteBatch.DrawString(
                PositionFont,
                "Blow up (x)",
                new Vector2(830, 640),
                Color.Tomato,
                0,
                FontOrigin,
                0.7f,
                SpriteEffects.None,
                0.5f);

            FontOrigin = PositionFont.MeasureString("Re-build (z)") / 2;
            _spriteBatch.DrawString(
                PositionFont,
                "Re-build (z)",
                new Vector2(830, 665),
                Color.Green,
                0,
                FontOrigin,
                0.7f,
                SpriteEffects.None,
                0.5f);

            FontOrigin = PositionFont.MeasureString("Life") / 2;
            _spriteBatch.DrawString(
                PositionFont,
                "Life",
                new Vector2(830, 690),
                Color.Red,
                0,
                FontOrigin,
                0.7f,
                SpriteEffects.None,
                0.5f);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            Animate();

            WallCollisionVerifier wallCollistionVer = (WallCollisionVerifier)Game.Services.GetService(typeof(WallCollisionVerifier));
            KeyboardState currentState = Keyboard.GetState();

            var collisionVerifier = (WallCollisionVerifier)Game.Services.GetService(typeof(WallCollisionVerifier));
            
            if (currentState.GetPressedKeys().Contains(Keys.Left))
            {
                if (collisionVerifier.CanMoveLeft(CollisionRectangle)) _currentPosition.X -= MovementStep;                
            }

            if (currentState.GetPressedKeys().Contains(Keys.Right))
            {
                if (collisionVerifier.CanMoveRight(CollisionRectangle)) _currentPosition.X += MovementStep;                
            }
            if (currentState.GetPressedKeys().Contains(Keys.Up))
            {
                if (collisionVerifier.CanMoveUp(CollisionRectangle)) _currentPosition.Y -= MovementStep;                
            }
            if (currentState.GetPressedKeys().Contains(Keys.Down))
            {
                if (collisionVerifier.CanMoveDown(CollisionRectangle)) _currentPosition.Y += MovementStep;                
            }

            if (currentState.GetPressedKeys().Contains(Keys.X) && DateTime.Now.Subtract(_lastBomb).TimeOfDay > BombReloadTime)
            {
                MarkNearWalls(WallModifier.Destroy);
                Game.Content.Load<SoundEffect>("boom").Play();
                _lastBomb = DateTime.Now.TimeOfDay;
            }

            if (currentState.GetPressedKeys().Contains(Keys.Z) && DateTime.Now.Subtract(_lastBuild).TimeOfDay > BuildReloadTime)
            {
                MarkNearWalls(WallModifier.Build);
                Game.Content.Load<SoundEffect>("build").Play();
                _lastBuild = DateTime.Now.TimeOfDay;
            }

            CollisionRectangle = new Rectangle((int)_currentPosition.X, (int)_currentPosition.Y, 40, 40);

            MarkNearWalls(WallModifier.None);

            int bombReloadPercentage = (int)((DateTime.Now.Subtract(_lastBomb).TimeOfDay.TotalSeconds / BombReloadTime.TotalSeconds) * 100);
            int buildReloadPercentage = (int)((DateTime.Now.Subtract(_lastBuild).TimeOfDay.TotalSeconds / BuildReloadTime.TotalSeconds) * 100);
            _buildProgressBar.value = buildReloadPercentage;
            _bombProgressBar.value = bombReloadPercentage;
            _lifeProgressBar.value = _life;
            _buildProgressBar.Update(gameTime);
            _bombProgressBar.Update(gameTime);
            _lifeProgressBar.Update(gameTime);

            CheckIfKilled();

            base.Update(gameTime);
        }

        private void CheckIfKilled()
        {
            Miner[] miners = (Miner [])base.Game.Services.GetService(typeof(Miner[]));
            Score score = (Score)base.Game.Services.GetService(typeof(Score));

            foreach (Miner miner in miners)
            {
                if (miner.CollisionRectangle.Intersects(CollisionRectangle))
                {
                    if (DateTime.Now.Subtract(_lastHurt).TimeOfDay > HurtOffset)
                    {
                        _lastHurt = DateTime.Now.TimeOfDay;
                        _life--;
                        Game.Content.Load<SoundEffect>("hurt").Play();

                        if (_life <= 0)
                        {
                            Walkers.WereKilled = true;
                            score.DecreaseScore(score.ScoreValue);
                        }
                    }
                }
            }
        }

        private void MarkNearWalls(WallModifier wallModifier)
        {
            Rectangle changeRectangle = new Rectangle(CollisionRectangle.X, CollisionRectangle.Y, CollisionRectangle.Width, CollisionRectangle.Height);
            changeRectangle.Width += 3; changeRectangle.Height += 3;
            changeRectangle.X -= 2;
            changeRectangle.Y -= 2;

            Labyrinth labyrinth = (Labyrinth)Game.Services.GetService(typeof(Labyrinth));
            for (int i = 0; i < labyrinth.Width; i++)
            {
                for (int k = 0; k < labyrinth.Height; k++)
                {
                    labyrinth[i, k].Color = Color.Gray;
                    if (labyrinth[i, k].CollisionRectangle.Intersects(changeRectangle))
                    {
                        labyrinth[i, k].Color = Color.Red;
                        if (wallModifier == WallModifier.Destroy)
                        {
                            labyrinth.Remove(i, k);
                        }                        
                    }

                    if (wallModifier == WallModifier.Build)
                    {
                        if (labyrinth[i, k].CollisionRectangleForBuild.Intersects(changeRectangle))
                        {
                            labyrinth[i, k].Color = Color.Green;
                            labyrinth.Build(i, k);
                        }
                    }
                }
            }
        }

        private void Animate()
        {
            if (DateTime.Now.Subtract(_lastAnimationTime).TimeOfDay > TargetElapsedTime)
            {
                _lastAnimationTime = new TimeSpan(DateTime.Now.Ticks);

                ++_currentFrame.X;
                if (_currentFrame.X >= _sheetSize.X)
                {
                    _currentFrame.X = 0;
                    ++_currentFrame.Y;
                    if (_currentFrame.Y >= _sheetSize.Y)
                        _currentFrame.Y = 0;
                }
            }
        }
    }

    enum WallModifier
    {
        None, 
        Build,
        Destroy
    }
}
