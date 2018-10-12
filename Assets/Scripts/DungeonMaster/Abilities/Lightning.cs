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
            Vector3Int offset = (Vector3Int)targetInfo;
            Vector3Int targetPos = caster.Position + (Vector3Int)targetInfo;
            List<Result> results = new List<Result>();
            Unit hitUnit;
            Block hitBlock;

            if(offset.magnitude > 6)
            {
                results.Add(new Result(Result.ResultType.InvalidAction, "too far", "out of range", null));
                return results;
            }

            caster.HasActed = true;
            caster.Stamina.Current -= 3;

            for (int i = battle.map.Shape[2] - 1; i >= 0; i--) //start at highest z-level and go down
            {
                Vector3Int hitPos = new Vector3Int(targetPos.x, targetPos.y, i);
                hitBlock = battle.map.BlockAt(hitPos);


                if (hitBlock.Conductive)
                {
                    List<Vector3Int> adjacent = Map.GetAdjacent(hitPos);
                    foreach (var adj in adjacent)
                    {
                        var secondaryBlock = battle.map.BlockAt(adj);
                        results.AddRange(secondaryBlock.StruckByLightning());
                        var secondaryUnit = battle.units.FirstOrDefault(u => u.Position == adj);
                        if (secondaryUnit != null)
                        {
                            Deck outcome = new Deck();

                            Card struck = new Card(Card.CardType.Hit, secondaryUnit.Name + " struck by lightning");
                            outcome.AddCards(struck, secondaryUnit.Position.z);

                            Card dodge = new Card(Card.CardType.Miss, secondaryUnit.Name + " dodges the lightning");
                            outcome.AddCards(dodge, secondaryUnit.Stamina.Current);

                            var drawn = outcome.Draw(fated_outcome);
                            switch (drawn.Type)
                            {
                                case Card.CardType.Hit:
                                    Result hitResult = new Result(Result.ResultType.Deck, "hit", drawn.Description, null);
                                    hitResult.OutcomeDeck = outcome;
                                    secondaryUnit.TakeDamage(3);
                                    break;
                                case Card.CardType.Armor:
                                    break;
                                case Card.CardType.Miss:
                                    Result missResult = new Result(Result.ResultType.Deck, "miss", drawn.Description, null);
                                    missResult.OutcomeDeck = outcome;
                                    break;
                                default:
                                    break;
                            }
                        }

                    }

                }

                if (hitBlock.Solid)
                {
                    results.Add(new Result(Result.ResultType.Generic, "lightning",
                        "Lightning bolt strikes " + hitBlock.Name + " at " + hitPos, null));
                    return results;
                }

                results.AddRange(hitBlock.StruckByLightning());
                hitUnit = battle.units.FirstOrDefault(u => u.Position == hitPos);
                if (hitUnit != null)
                {
                    //struck by lightning
                    Deck outcome = new Deck();

                    Card struck = new Card(Card.CardType.Hit, hitUnit.Name + " struck by lightning");
                    outcome.AddCards(struck, hitUnit.Position.z);

                    Card dodge = new Card(Card.CardType.Miss, hitUnit.Name + " dodges the lightning");
                    outcome.AddCards(dodge, hitUnit.Stamina.Current);

                    var drawn = outcome.Draw(fated_outcome);
                    switch (drawn.Type)
                    {
                        case Card.CardType.Hit:
                            Result hitResult = new Result(Result.ResultType.Deck, "hit", drawn.Description, null);
                            hitResult.OutcomeDeck = outcome;
                            hitUnit.TakeDamage(7);
                            break;
                        case Card.CardType.Armor:
                            break;
                        case Card.CardType.Miss:
                            Result missResult = new Result(Result.ResultType.Deck, "miss", drawn.Description, null);
                            missResult.OutcomeDeck = outcome;
                            break;
                        default:
                            break;
                    }
                }
            }

            throw new NotImplementedException(); //should never be here, will always hit something

        }
    }
}
