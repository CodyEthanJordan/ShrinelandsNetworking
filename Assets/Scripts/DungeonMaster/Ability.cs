using Assets.Scripts.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.DungeonMaster
{
    public abstract class Ability
    {
        public string Name;
        public string Description;

        public abstract bool CanBeUsed(Battle battle, Unit caster);
        public abstract List<Result> UseAbility(Battle battle, Unit caster, object targetInfo);

        public static object ParseTarget(Battle battle, Unit caster, string target)
        {
            switch(target.ToLower())
            {
                case "n":
                    return new Vector3Int(0, 1, 0);
                case "s":
                    return new Vector3Int(0, -1, 0);
                case "e":
                    return new Vector3Int(1, 0, 0);
                case "w":
                    return new Vector3Int(-1, 0, 0);
            }

            var targetUnit = battle.units.FirstOrDefault(u => u.Name.Equals(target, StringComparison.CurrentCultureIgnoreCase));
            if(targetUnit != null)
            {
                return caster.Position - targetUnit.Position;
            }

            return null;
        }
    }
}
