using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.DungeonMaster
{
    public class Block
    {
        public Guid ID;

        public string Name;
        public bool Solid;
        public int MoveCost;

        Block()
        {
            ID = Guid.NewGuid();
        }

        Block(string name, bool solid, int moveCost)
        {
            this.Name = name;
            this.Solid = solid;
            this.MoveCost = moveCost;
        }

        public static Block GetDebugDirt()
        {
            return new Block("Dirt", true, 1);
        }

        public static Block GetDebugAir()
        {
            return new Block("Air", false, 1);
        }
    }
}
