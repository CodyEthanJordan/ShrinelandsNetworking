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
        public bool Flugen;
        public bool Conductive;
        public int MoveCost;

        public Block()
        {
            //ID = Guid.NewGuid();
        }

        public Block(string name, bool solid, int moveCost)
        {
            //this.ID = Guid.NewGuid();
            this.Name = name;
            this.Solid = solid;
            this.MoveCost = moveCost;
            Conductive = false;
            Flugen = false;
        }

        public static Block GetDebugDirt()
        {
            return new Block("dirt", true, 1);
        }

        public static Block GetDebugAir()
        {
            return new Block("air", false, 0);
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
                    lava.Flugen = true;
                    return lava;
                case ']':
                    return new Block("tree", true, 1);
                case '%':
                    return new Block("shrubbery", false, 1);
                case '~':
                    var slime = new Block("slime", false, 1);
                    slime.Flugen = true;
                    slime.Conductive = true;
                    return slime;
                default:
                    return Block.GetDebugAir();
            }
        }

        internal List<Result> StruckByLightning()
        {
            List<Result> results = new List<Result>();
            if(this.Name == "slime")
            {
                results.Add(new Result(Result.ResultType.Generic, "evaporated", "slime evaporated by lightning", null));
                this.Become(Block.GetDebugAir());
                return results;
            }
            else
            {
                return results;
            }
        }

        private void Become(Block block)
        {
            this.Name = block.Name;
            this.MoveCost = block.MoveCost;
            this.Conductive = block.Conductive;
            this.Flugen = block.Flugen;
            this.Solid = block.Solid;
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
                case "slime":
                    return '~';
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
                results.Add(new Result(Result.ResultType.Generic, "damage",
                    unit.Name + " took 5 damage from lava",
                    new Update(unit))); //TODO: effect does not work right, needs copy
            }

            return results;
        }

        internal IEnumerable<Result> ApplyStartTurnEffects(Battle b, Unit unit)
        {
            throw new NotImplementedException();
        }
    }
}
