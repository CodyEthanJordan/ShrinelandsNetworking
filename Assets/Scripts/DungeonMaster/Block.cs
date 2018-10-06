using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.DungeonMaster
{
    public class Block
    {
        public string Name;
        public bool Solid;
        public int MoveCost;

        Block()
        {
            //ID = Guid.NewGuid();
        }

        Block(string name, bool solid, int moveCost)
        {
            //this.ID = Guid.NewGuid();
            this.Name = name;
            this.Solid = solid;
            this.MoveCost = moveCost;
        }

        public static Block GetDebugDirt()
        {
            return new Block("dirt", true, 1);
        }

        public static Block GetDebugAir()
        {
            return new Block("air", false, 1);
        }

        public static Block FromString(char v)
        {
            switch(v)
            {
                case '#':
                    return new Block("stone", true, 1);
                case 'B':
                    return new Block("bedrock", true, 1);
                case '.':
                    return Block.GetDebugAir();
                case '!':
                    return new Block("lava", false, 1);
                case ']':
                    return new Block("tree", true, 1);
                case '%':
                    return new Block("shrubbery", false, 1);
                default:
                    return Block.GetDebugAir();
            }
        }
    }
}
