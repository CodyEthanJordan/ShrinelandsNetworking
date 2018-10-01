﻿using System;
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

        public static Battle GetDebugBattle()
        {
            var defaultBattle = new Battle();

            defaultBattle.map = Map.GetDebugMap();

            defaultBattle.sides.Add(new Side("The Player", "#0000FF"));
            defaultBattle.sides.Add(new Side("The Foe", "#FF0000"));

            defaultBattle.units.Add(Unit.GetDefaultDude("Charlie", defaultBattle.sides[0].ID,
                new Vector3Int(3, 3, 3)));
            defaultBattle.units.Add(Unit.GetDefaultDude("Robby", defaultBattle.sides[0].ID,
                new Vector3Int(3, 5, 3)));

            defaultBattle.units.Add(Unit.GetDefaultDude("JJ", defaultBattle.sides[1].ID,
                new Vector3Int(5, 5, 3)));
            defaultBattle.units.Add(Unit.GetDefaultDude("Zach", defaultBattle.sides[1].ID,
                new Vector3Int(5, 6, 3)));

            defaultBattle.currentSide = defaultBattle.sides[0];

            return defaultBattle;
        }

        public void MakeMove(Guid unitID, Map.Direction direction)
        {

        }

        public List<Result> MoveUnit(Guid unitID, Vector3Int target)
        {
            var unit = units.First(u => u.ID == unitID);
            var standingOn = map.BlockAt(unit.Position + new Vector3Int(0, 0, -1));
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
                var result = new Result(Result.ResultType.Movement, "Move", unit.Name + " moved to " + target,
                    new Effect(unit));
                return results;
            }
        }

        public void HandleResult(Result result)
        {
            switch (result.Type)
            {
                case Result.ResultType.Movement:
                    var unit = units.First(u => u.ID == result.UnitAffected.ID);
                    unit.HandleResult(result);
                    break;
            }
        }

        internal List<Result> EndTurn(Guid sideID)
        {
            if(sideID != currentSide.ID)
            {
                //how did we end up here
                //TODO: logging
                return null;
            }

            List<Result> results = new List<Result>();

            foreach (var unit in units)
            {
                results.Add(unit.EndTurn());
            }

            return results;
        }

        internal void HandleResults(List<Result> results)
        {
            foreach (var result in results)
            {
                HandleResult(result);
            }
        }
    }
}
