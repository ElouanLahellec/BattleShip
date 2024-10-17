using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Security.Principal;

namespace BattleShip.API
{
    public class Board
    {
        public List<List<char>> grid { get; set; }
       
        public Board() 
        {
            grid = new List<List<char>>();
        }

        public void init(int width, int height)
        {
            for (int i = 0; i < width; i++)
            {
                List<char> line = new List<char>();
                for (int j = 0; j < height; j++)
                {
                    line.Add('\0');
                }
                grid.Add(line);
            }
        }

        public List<List<char>> PlaceRdmBoats()
        {
            int[] BoatSizes = { 5, 4, 3, 3, 2 };
            char[] BoatNames = { 'A', 'B', 'C', 'D', 'E' };
            Random random = new Random();

            for (int i = 0; i < BoatSizes.Length; i++)
            {
                bool place = false;

                while (!place)
                {
                    int size = BoatSizes[i];
                    char boat = BoatNames[i];
                    bool horizontal = random.Next(2) == 0;
                    int line = random.Next(grid.Count);
                    int column = random.Next(grid[0].Count);

                    if (CanPlaceBoat(line, column, size, horizontal))
                    {
                        PlaceBoat(line, column, size, horizontal, boat);
                        place = true;
                    }
                }
            }
            return grid;
        }
        private List<List<char>> PlaceBoat(int x, int y, int size, bool horizontal, char boat)
        {
            if (horizontal)
            {
                for (int i = 0; i < size; i++)
                {
                    grid[x][y + i] = boat;
                }
            }
            else
            {
                for (int i = 0; i < size; i++)
                {
                    grid[x + i][y] = boat;
                }
            }
            return grid;
        }

        private bool CanPlaceBoat(int x, int y, int size, bool horizontal)
        {
            if (horizontal)
            {
                if (y + size > grid[0].Count)
                    return false;

                for (int i = 0; i < size; i++)
                {
                    if (grid[x][y + i] != '\0')
                        return false;
                }
            }
            else
            {
                if (x + size > grid.Count)
                    return false;

                for (int i = 0; i < size; i++)
                {
                    if (grid[x + i][y] != '\0')
                        return false;
                }
            }
            return true;
        }

        public bool IsHit(int x, int y)
        {
            return grid[x][y] != '\0';
        }
    }
}
