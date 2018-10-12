using Assets.Scripts.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.DungeonMaster.Abilities
{
    public class SlimeSpawn : Ability
    {
        public SlimeSpawn()
        {
            Name = "SlimeSpawn";
            Description = "Spawns slime all around caster";
        }

        public override bool CanBeUsed(Battle battle, Unit caster)
        {
            if (caster.Stamina.Current >= 4)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override List<Result> UseAbility(Battle battle, Unit caster, object targetInfo, int? fated_outcome=null)
        {
            caster.Stamina.Current -= 4;
            int spawned = 0;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    var pos = caster.Position + new UnityEngine.Vector3Int(i, j, 0);
                    var block = battle.map.BlockAt(pos);
                    if (block.Name == "air")
                    {
                        Block slime = new Block("slime", false, 1)
                        {
                            Flugen = true,
                            Conductive = true,
                        };
                        battle.map.Blocks[pos.x][pos.y][pos.z] = slime;
                        spawned++;
                    }
                }
            }
            List<Result> results = new List<Result>();
            results.Add(new Result(Result.ResultType.Generic, "slimespawn",
                "Spawned " + spawned.ToString() + " slime", null));

            return results;
        }
    }
}
