using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Networking;
using UnityEngine;

namespace Assets.Scripts.DungeonMaster.Abilities
{
    public class Lightning : Ability
    {
        public Lightning()
        {
            Name = "lightning";
            Description = "Call down a bolt of lightning from above. Contests z-level vs stamina to dodge, dealing 7 damage. Cost 3 stamina";
        }

        public override bool CanBeUsed(Battle battle, Unit caster)
        {
            return caster.Stamina.Current >= 3;
        }

        public override List<Result> UseAbility(Battle battle, Unit caster, object targetInfo, int? fated_outcome = null)
        {
            Vector3Int targetPos = caster.Position + (Vector3Int)targetInfo;
            List<Result> results = new List<Result>();
            Unit hitUnit;
            Block hitBlock;

            for (int i = battle.map.Shape[2]-1; i <= 0; i--) //start at highest z-level and go down
            {
                Vector3Int hitPos = new Vector3Int(targetPos.x, targetPos.y, i);
                hitBlock = battle.map.BlockAt(hitPos);
                if(hitBlock.Solid)
                {
                    results.Add(new Result(Result.ResultType.Generic, "lightning",
                        "Lightning bolt strikes " + hitBlock.Name + " at " + hitPos, null));
                    return results;
                }
                hitUnit = battle.units.FirstOrDefault(u => u.Position == hitPos);
                if(hitUnit != null)
                {
                    //struck by lightning
                    Deck outcome = new Deck();
                    
                    Card card = new Card(Card.CardType.Hit, "");
                    card.DoThing((Battle b, Unit c, Unit h) =>
                    {
                        h.TakeDamage(7);
                        return new List<Result>()
                        {
                            new Result(Result.ResultType.G)
                        }
                    });
                }
            }

            throw new NotImplementedException(); //should never be here, will always hit something

        }
    }
}
