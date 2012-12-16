using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.Objects.Labyrinth;
using Microsoft.Xna.Framework;

namespace GameLibrary.Algorithms.Labyrinth
{
    public class LabyrinthBuilder
    {
        private const int Offset = 20;

        public static GameLibrary.Objects.Labyrinth.Labyrinth Build(Game game, int width, int height)
        {
            var randomGenerator = new Random((int) DateTime.Now.Ticks);
            var data = new LabyrinthUnit[width, height];
            
            //create random walls
            for(int i=0;i<width;i++)
            {
                for (int k = 0; k < height; k++)
                {
                    WallType wall = BuildRandomWall(randomGenerator);
                    data[i, k] = new LabyrinthUnit(game, i * LabyrinthUnit.Length + Offset, k * LabyrinthUnit.Length + Offset, i, k, wall);
                }
            }
            
            //try to get through and destroy wall if needed


            return new Objects.Labyrinth.Labyrinth(game, data);
        }

        private static WallType BuildRandomWall(Random randomGenerator)
        {
            var randomNumber = randomGenerator.Next() % 51;

            if (randomNumber < 10) return WallType.Left;
            if (randomNumber < 20) return WallType.Top;
            if (randomNumber < 30) return WallType.Right;
            
            return WallType.Bottom;            
        }
    }
}
