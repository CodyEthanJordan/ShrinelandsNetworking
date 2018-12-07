using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Networking;
using UnityEngine;

namespace Assets.Scripts.DungeonMaster.Buffs
{
    public class StrikeFromShadows : Buff
    {

        public StrikeFromShadows()
        {
            this.Name = "Strike From Shadows";
        }

        public override List<Result> UnitMoved(Battle battle, Unit unit, Vector3Int oldPos, Vector3Int position)
        {
            var results = new List<Result>();
            int i = unit.Buffs.FindIndex(b => b.Name == this.Name);
            unit.Buffs[i] = null;
            return results;
        }

        public override List<Result> UnitUsedAbility(Battle battle, Unit unit, Ability ability)
        {
            var results = new List<Result>();
            int i = unit.Buffs.FindIndex(b => b.Name == this.Name);
            unit.Buffs[i] = null;
            return results;
        }
    }
}
