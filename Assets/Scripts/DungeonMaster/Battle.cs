using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public Battle()
        {
            ID = Guid.NewGuid();
        }

        public bool IsPassable(Vector3Int pos)
        {
            if(units.Exists(u => u.Position == pos))
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

        public List<Vector3Int> GetValidMovements(Guid unitID)
        {
            List<Vector3Int> allowedDestinations = new List<Vector3Int>();
            var unit = units.First(u => u.ID == unitID);

            foreach (var dir in Map.CardinalDirections)
            {
                var dest = unit.Position + dir;
                if(IsPassable(dest))
                {
                    allowedDestinations.Add(dest);
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
                new Vector3Int(3,3,3)));
            defaultBattle.units.Add(Unit.GetDefaultDude("Robby", defaultBattle.sides[0].ID,
                new Vector3Int(3,5,3)));

            defaultBattle.units.Add(Unit.GetDefaultDude("JJ", defaultBattle.sides[1].ID,
                new Vector3Int(5,5,3)));
            defaultBattle.units.Add(Unit.GetDefaultDude("Zach", defaultBattle.sides[1].ID,
                new Vector3Int(5,6,3)));

            return defaultBattle;
        }
        
        public void MakeMove(Guid unitID, Map.Direction direction)
        {

        }
    }
}
