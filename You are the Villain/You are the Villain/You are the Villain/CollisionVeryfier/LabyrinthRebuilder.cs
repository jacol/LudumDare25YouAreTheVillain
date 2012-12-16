using GameLibrary.Objects.Labyrinth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using You_are_the_Villain.GameObjects;

namespace You_are_the_Villain.CollisionVeryfier
{
    internal static class LabyrinthRebuilder
    {
        internal static void MakeSureMineHasAccessToVillage(Labyrinth labyrinth, Mine mine, Village village)
        {
            int xFrom, yFrom, xTo, yTo;

            if (mine.LabyrinthCorX >= village.LabyrinthCorX)
            {
                xFrom = village.LabyrinthCorX;
                xTo = mine.LabyrinthCorX;
            }
            else
            {
                xFrom = mine.LabyrinthCorX;
                xTo = village.LabyrinthCorX;                
            }

            if (mine.LabyrinthCorY >= village.LabyrinthCorY)
            {
                yFrom = village.LabyrinthCorY;
                yTo = mine.LabyrinthCorY;
            }
            else
            {                
                yFrom = mine.LabyrinthCorY;
                yTo = village.LabyrinthCorY;                
            }

            for (int i = xFrom; i <= xTo; i++)
            {
                labyrinth.Remove(i, yFrom);
                labyrinth.Remove(i, yTo);
            }

            for (int k = yFrom; k <= yTo; k++)
            {
                labyrinth.Remove(xTo, k);
                labyrinth.Remove(xFrom, k);
            }

            

        }
    }
}
