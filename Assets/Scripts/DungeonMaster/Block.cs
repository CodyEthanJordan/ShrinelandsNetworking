using Assets.Scripts.Networking;
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
        public bool Buoyant;
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
                    var lava = new Block("lava", false, 1);
                    lava.Buoyant = true;
                    return lava;
                case ']':
                    return new Block("tree", true, 1);
                case '%':
                    return new Block("shrubbery", false, 1);
                default:
                    return Block.GetDebugAir();
            }
        }

        public char GetChar()
        {
            switch(Name)
            {
                case "stone":
                    return '#';
                case "lava":
                    return '!';
                case "air":
                    return '.';
                case "tree":
                    return ']';
                case "shrubbery":
                    return '%';
                case "bedrock":
                    return 'B';
                default:
                    return '.';
            }
        }

        public List<Result> ApplyBlockEffects(Unit unit)
        {
            List<Result> results = new List<Result>();
            if(Name == "lava")
            {
                unit.TakeDamage(5);
                results.Add(new Result(Result.ResultType.GenericUpdate, "damage",
                    unit.Name + " took 5 damage from lava",
                    new Effect(unit))); //TODO: effect does not work right, needs copy
            }

            return results;
        }
    }
}
