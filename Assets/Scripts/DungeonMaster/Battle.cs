using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.DungeonMaster
{
    public class Battle
    {
        public Guid ID { get; private set; }

        public Map map;
        public List<Side> sides = new List<Side>();
        public List<Unit> units = new List<Unit>();

        public Battle()
        {
            ID = Guid.NewGuid();
        }

        public static Battle GetDebugBattle()
        {
            var defaultBattle = new Battle();

            defaultBattle.map = Map.GetDebugMap();

            defaultBattle.sides.Add(new Side("The Player"));
            defaultBattle.sides.Add(new Side("The Foe"));

            defaultBattle.units.Add(new Unit("Charlie", defaultBattle.sides[0].ID,
                new Vector3Int(0,0,3)));
            defaultBattle.units.Add(new Unit("Richard", defaultBattle.sides[0].ID,
                new Vector3Int(1,0,3)));

            defaultBattle.units.Add(new Unit("JJ", defaultBattle.sides[1].ID,
                new Vector3Int(5,5,3)));
            defaultBattle.units.Add(new Unit("Zach", defaultBattle.sides[1].ID,
                new Vector3Int(5,6,3)));

            return defaultBattle;
        }

        public void MakeMove(Guid unitID, Map.Direction direction)
        {

        }
    }
}
