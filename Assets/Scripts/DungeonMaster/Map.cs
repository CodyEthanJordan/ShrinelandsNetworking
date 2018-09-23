using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.DungeonMaster
{
    public class Map
    {
        public List<List<List<Block>>> Blocks = new List<List<List<Block>>>();

        public List<int> Shape
        {
            get
            {
                var shape = new List<int>();
                shape.Add(Blocks.Count);
                shape.Add(Blocks[0].Count);
                shape.Add(Blocks[0][0].Count);
                return shape;
            }
        }

        public Map(int x, int y, int z)
        {
            for (int i = 0; i < x; i++)
            {
                Blocks.Add(new List<List<Block>>());
                for (int j = 0; j < y; j++)
                {
                    Blocks[i].Add(new List<Block>());
                    for (int k = 0; k < z; k++)
                    {
                        Blocks[i][j].Add(Block.GetDebugAir());
                    }
                }
            }
        }

        public Block BlockAt(Vector3Int pos)
        {
            return Blocks[pos.x][pos.y][pos.z];
        }

        public static Map GetDebugMap()
        {
            var debugMap = new Map(15, 10, 6);

            for (int i = 0; i < debugMap.Shape[0]; i++)
            {
                for (int j = 0; j < debugMap.Shape[1]; j++)
                {
                    for (int k = 0; k < debugMap.Shape[2]/2; k++)
                    {
                        debugMap.Blocks[i][j][k] = Block.GetDebugDirt();
                    }
                }
            }

            return debugMap;
        }

        public enum Direction
        {
            North, South, East, West, Up, Down
        }

        public static readonly List<Vector3Int> CardinalDirections = new List<Vector3Int>()
        {
            Vector3Int.up, Vector3Int.right, Vector3Int.down, Vector3Int.left
        };

        public int Distance(Vector3Int position, Vector3Int target)
        {
            var difference = target - position;
            return Math.Abs(difference.x) + Math.Abs(difference.y) + Math.Abs(difference.z);
        }
    }
}
