using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace You_are_the_Villain.Managers
{
    class MenuManager : DrawableGameComponent
    {
        private int _screenHeight, _screenWidth;
        private Score _score;

        public Border Border { get; private set; }

        internal MenuManager(Game game, Score score, int screenWidth, int screenHeight)
            :base(game)
        {
            _score = score;
            _screenHeight = screenHeight;
            _screenWidth = screenWidth;
            Border = new Border(base.Game, _screenWidth - 45, _screenHeight - 95);
        }

        public override void Initialize()
        {            
            base.Game.Components.Add(Border);
            base.Game.Components.Add(_score);

            base.Initialize();
        }
    }
}
