using GameLibrary.Algorithms.Labyrinth;
using GameLibrary.Objects.Labyrinth;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using You_are_the_Villain.CollisionVeryfier;
using You_are_the_Villain.GameObjects;

namespace You_are_the_Villain.Managers
{
    internal class GameObjectsManager
    {
        private const int MinerCount = 4;

        private Game _game;
        private int _screenHeight, _screenWidth;

        internal GameObjectsManager(Game game, int screenWidth, int screenHeight)
        {
            _game = game;
            _screenHeight = screenHeight;
            _screenWidth = screenWidth;
        }

        internal void Initialize()
        {            
            var labyrinth = InitializeLabyrinth();
            var menuManager = InitializeGameMenu();
            
            var village = InitializeVillage(labyrinth);
            var mine = InitializeMine(labyrinth);

            LabyrinthRebuilder.MakeSureMineHasAccessToVillage(labyrinth, mine, village);

            var miners = InitializeMiners(labyrinth, village, mine);
            var monster = InitializeMonster(labyrinth);

            InitializeServices(menuManager, labyrinth);
        }

        private void InitializeServices(MenuManager menuManager, Labyrinth labyrinth)
        {
            _game.Services.AddService(typeof(WallCollisionVerifier), new WallCollisionVerifier(menuManager.Border, labyrinth));
            _game.Services.AddService(typeof(LabyrinthPathFinder), new LabyrinthPathFinder(labyrinth));
        }        

        private Monster InitializeMonster(Labyrinth labyrinth)
        {
            Monster monster = new Monster(_game, labyrinth[0, 0].TopLeftPosition, 0, 0);
            _game.Components.Add(monster);
            _game.Services.AddService(typeof(Monster), monster);

            return monster;
        }

        private Miner [] InitializeMiners(Labyrinth labyrinth, Village village, Mine mine)
        {
            Miner[] miners = new Miner[MinerCount];

            for (int i = 0; i < MinerCount; i++)
            {
                miners[i] = new Miner(_game, village.StartPosition, village.LabyrinthCorX, village.LabyrinthCorY, mine.LabyrinthCorX, mine.LabyrinthCorY, new TimeSpan(0, 0, i * 10));
                _game.Components.Add(miners[i]);
            }

            _game.Services.AddService(typeof(Miner [] ), miners);

            return miners;
        }

        private Mine InitializeMine(Labyrinth labyrinth)
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            int x = random.Next(labyrinth.Width);
            int y = random.Next(labyrinth.Height);

            var labyrinthUnit = labyrinth[x, y];

            Mine mine = new Mine(_game, labyrinthUnit.TopLeftPosition, x, y);
            _game.Components.Add(mine);
            _game.Services.AddService(typeof(Mine), mine);

            return mine;
        }

        private Village InitializeVillage(Labyrinth labyrinth)
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            int x = random.Next(labyrinth.Width);
            int y = random.Next(labyrinth.Height);

            var labyrinthUnit = labyrinth[x, y];

            Village village = new Village(_game, labyrinthUnit.TopLeftPosition, x, y);
            _game.Components.Add(village);
            _game.Services.AddService(typeof(Village), village);

            return village;
        }

        private MenuManager InitializeGameMenu()
        {
            Score score = new Score(_game, _screenWidth, _screenHeight, 1000);
            _game.Services.AddService(typeof(Score), score);

            MenuManager menuManager = new MenuManager(_game, score, _screenWidth, _screenHeight);
            _game.Components.Add(menuManager);

            return menuManager;
        }

        private Labyrinth InitializeLabyrinth()
        {
            int labWidth = _screenWidth / LabyrinthUnit.Length;
            int labHeight = (_screenHeight / LabyrinthUnit.Length) - 1;

            var labyrinth = LabyrinthBuilder.Build(_game, labWidth, labHeight);
            _game.Components.Add(labyrinth);
            _game.Services.AddService(typeof(Labyrinth), labyrinth);

            return labyrinth;
        }
    }
}
