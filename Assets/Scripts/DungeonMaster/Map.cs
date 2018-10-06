using System;
using System.Collections.Generic;
using System.IO;
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

        public static Vector3Int VectorFromDirection(Direction dir)
        {
            switch(dir)
            {
                case Direction.North:
                    return new Vector3Int(0, 1, 0);
                    break;
                case Direction.South:
                    return new Vector3Int(0, -1, 0);
                    break;
                case Direction.East:
                    return new Vector3Int(1, 0, 0);
                    break;
                case Direction.West:
                    return new Vector3Int(-1, 0, 0);
                    break;
                case Direction.Up:
                    return new Vector3Int(0, 0, 1);
                    break;
                case Direction.Down:
                    return new Vector3Int(0, 0, -1);
                    break;
            }
            return Vector3Int.zero; //should never happen, TODO: log and test
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

        public static bool IsRookMove(Vector3Int a, Vector3Int b)
        {
            var diff = a - b;
            int zeroDimensions = (diff.x == 0 ? 1 : 0) +
                                 (diff.y == 0 ? 1 : 0) +
                                 (diff.z == 0 ? 1 : 0);
            return zeroDimensions >= 2;

        }

        public static Map FromString(string mapString)
        {
            var sr = new StringReader(mapString);
            string first = sr.ReadLine();
            var size = first.Split(' ').Select(s => int.Parse(s)).ToList();
            Map m = new Map(size[0], size[1], size[2]);

            for (int k = 0; k < size[2]; k++)
            {
                for (int j = size[1]-1; j >= 0; j--)
                {
                    string line = sr.ReadLine();
                    for (int i = 0; i < size[0]; i++)
                    {
                        m.Blocks[i][j][k] = Block.FromString(line[i]);
                    }
                }
                string blankLine = sr.ReadLine();
            }

            return m;
        }
    }
}
