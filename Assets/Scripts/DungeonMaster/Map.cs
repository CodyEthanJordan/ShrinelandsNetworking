using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.DungeonMaster
{
    public class Map
    {
        public List<List<List<Block>>> Blocks;

        Map(int x, int y, int z)
        {
            Blocks = new List<List<List<Block>>>();
            for (int i = 0; i < x; i++)
            {
                Blocks[i] = new List<List<Block>>();
                for (int j = 0; j < y; j++)
                {
                    Blocks[i][j] = new List<Block>();
                    for (int k = 0; k < z; k++)
                    {
                        Blocks[i][j][k] = Block.GetDebugAir();
                    }
                }
            }
        }
    }
}
