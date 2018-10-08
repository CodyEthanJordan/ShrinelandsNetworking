using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Networking;
using Newtonsoft.Json;

using UnityEngine;

namespace Assets.Scripts.DungeonMaster
{
    public class Battle
    {
        public Guid ID;

        public Map map;
        public List<Side> sides = new List<Side>();
        public List<Unit> units = new List<Unit>();
        public Side currentSide;

        public Battle()
        {
            ID = Guid.NewGuid();
        }

        public bool IsPassable(Vector3Int pos)
        {
            if (units.Exists(u => u.Position == pos))
            {
                //someone is already there
                return false;
            }
            else if (map.BlockAt(pos).Solid)
            {
                //there is a block there
                return false;
            }
            else
            {
                //guess its fine?
                return true;
            }
        }

        public static bool IsAdjacent(Unit a, Unit b)
        {
            var diff = a.Position - b.Position;
            return Map.CardinalDirections.Contains(diff);
        }

        //public void BasicAttack(Guid unitID, Map.Direction dir, int? outcome_index = null)
        //{
        //    //TODO: add results?
        //    var unit = units.First(u => u.ID == unitID);
        //    var targetPos = unit.Position + Map.VectorFromDirection(dir);
        //    var targetedUnit = units.FirstOrDefault(u => u.Position == targetPos);
        //    if (targetedUnit == null)
        //    {
        //        return; //no one to attack
        //    }

        //    var card = Deck.BasicDraw(unit.Expertise.Current, 0, targetedUnit.Stamina.Current, outcome_index);

        //    switch (card)
        //    {
        //        case Deck.CardType.Hit:
        //            targetedUnit.HP.Current -= unit.Strength.Current;
        //            break;
        //        case Deck.CardType.Armor:
        //            throw new NotImplementedException();
        //            break;
        //        case Deck.CardType.Dodge:
        //            break;
        //    }
        //}

        public string GetAreaNear(Guid unitID)
        {
            var unit = units.Find(u => u.ID == unitID);
            throw new NotImplementedException();
        }

        public Dictionary<Vector3Int, Vector3Int> GetValidMovements(Guid unitID)
        {
            Dictionary<Vector3Int, Vector3Int> allowedDestinations = new Dictionary<Vector3Int, Vector3Int>();
            var unit = units.First(u => u.ID == unitID);

            foreach (var dir in Map.CardinalDirections)
            {
                var dest = unit.Position + dir;
                if (IsPassable(dest))
                {
                    allowedDestinations.Add(dir, dest);
                }
            }

            return allowedDestinations;
        }

        public List<Result> UseAbility(string unitName, string abilityName, string target)
        {
            var unit = units.FirstOrDefault(u => u.Name.Equals(unitName, StringComparison.CurrentCultureIgnoreCase));
            if(unit == null)
            {
                return null; //TODO: output
            }

            var ability = unit.Abilities.FirstOrDefault(a => a.Name.Equals(abilityName, StringComparison.CurrentCultureIgnoreCase));
            if (ability == null || !ability.CanBeUsed(this, unit))
            {
                return null; //TODO: output
            }

            var targetObject = Ability.ParseTarget(this, unit, target);

            return ability.UseAbility(this, unit, targetObject);
        }

        public static Battle GetDebugBattle()
        {
            var defaultBattle = new Battle();

            defaultBattle.map = Map.GetDebugMap();

            defaultBattle.sides.Add(new Side("The Player", "#0000FF"));
            defaultBattle.sides.Add(new Side("The Foe", "#FF0000"));

            defaultBattle.units.Add(Unit.GetDefaultDude("Charlie", defaultBattle.sides[0].ID,
                new Vector3Int(5, 3, 3)));
            defaultBattle.units.Add(Unit.GetDefaultDude("Robby", defaultBattle.sides[0].ID,
                new Vector3Int(3, 5, 3)));

            defaultBattle.units.Add(Unit.GetDefaultDude("JJ", defaultBattle.sides[1].ID,
                new Vector3Int(5, 5, 3)));
            defaultBattle.units.Add(Unit.GetDefaultDude("Zach", defaultBattle.sides[1].ID,
                new Vector3Int(5, 6, 3)));

            defaultBattle.currentSide = defaultBattle.sides[0];

            return defaultBattle;
        }

        public List<string> ShowMapByLevel()
        {
            var output = new List<string>();
            StringBuilder sb = new StringBuilder();

            for (int k = 0; k < map.Shape[2]; k++)
            {
                for (int j = map.Shape[1] - 1; j >= 0; j--)
                {
                    for (int i = 0; i < map.Shape[0]; i++)
                    {
                        var unitAt = units.FirstOrDefault(u => u.Position == new Vector3Int(i, j, k));
                        if (unitAt == null)
                        {
                            sb.Append(map.BlockAt(new Vector3Int(i, j, k)).GetChar());
                        }
                        else
                        {
                            sb.Append(unitAt.Name[0]);
                        }
                    }
                    sb.AppendLine("");
                }
                output.Add(sb.ToString());
                sb = new StringBuilder();
            }

            return output;
        }

        public List<Result> MakeMove(Guid unitID, Map.Direction direction)
        {
            var unit = units.First(u => u.ID == unitID);
            if (unit.SideID != currentSide.ID)
            {
                return null; //TODO: better message
            }
            return MoveUnit(unitID, unit.Position + Map.VectorFromDirection(direction));
        }

        public List<Result> MoveUnit(Guid unitID, Vector3Int target)
        {
            var unit = units.First(u => u.ID == unitID);
            var standingOn = map.StandingOn(unit);
            List<Result> results = new List<Result>();

            if (!IsPassable(target))
            {
                return null;
            }
            else if (standingOn.MoveCost > unit.Movement.Current)
            {
                return null;
            }
            else
            {
                unit.MoveTo(target, standingOn.MoveCost);
                var result = new Result(Result.ResultType.Generic, "Move", unit.Name + " moved to " + target,
                    new Update(unit));
                results.Add(result);

                //check block effects
                var newBlock = map.BlockAt(unit.Position);
                results.AddRange(newBlock.ApplyBlockEffects(unit));

                // not standing on anything solid and not swimming
                if (!map.StandingOn(unit).Solid && !newBlock.Buoyant)
                {
                    // TODO: fall damage
                    // TODO: falling shouldn't cost movement
                    results.AddRange(MoveUnit(unit.ID, unit.Position + new Vector3Int(0, 0, -1)));
                }

                return results;
            }
        }

        //public void HandleResult(Result result)
        //{
        //    switch (result.Type)
        //    {
        //        case Result.ResultType.Movement:
        //            break;
        //    }
        //}

        public List<Result> EndTurn()
        {
            //if (sideID != currentSide.ID)
            //{
            //    //how did we end up here
            //    //TODO: logging
            //    return null;
            //}

            List<Result> results = new List<Result>();


            foreach (var unit in units.FindAll(u => u.SideID == currentSide.ID))
            {
                results.AddRange(unit.EndTurn());
            }

            int i = sides.IndexOf(currentSide);
            i = (i + 1) % sides.Count;

            currentSide = sides[i];

            foreach (var unit in units.FindAll(u => u.SideID == currentSide.ID))
            {
                results.AddRange(unit.StartTurn());
            }

            return results;
        }

        public List<Result> CastMagicMissile(Unit caster, Vector3Int target)
        {
            int spellCost = 2;
            var results = new List<Result>();
            // first validate if this is possible
            if (caster.Stamina.Current < spellCost)
            {
                return null; //doesn't have suffecient stamina
            }

            if (!Map.IsRookMove(caster.Position, target))
            {
                return null; //magic missle can only be cast in straight lines
            }

            return results;
        }

        public string ShowBattle()
        {
            var output = new StringBuilder(map.ShowMap());

            foreach (var unit in units)
            {
                int i = map.GetStringIndex(unit.Position);
                output[i] = unit.Name[0];
            }

            return output.ToString();
        }

        internal void HandleResults(List<Result> results)
        {
            foreach (var result in results)
            {
                //HandleResult(result);
            }
        }
    }
}
