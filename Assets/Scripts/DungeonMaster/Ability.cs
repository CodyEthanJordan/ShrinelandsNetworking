using Assets.Scripts.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.DungeonMaster
{
    public abstract class Ability
    {
        public string Name;
        public string Description;

        public abstract bool CanBeUsed(Battle battle, Unit caster);
        public abstract List<Result> UseAbility(Battle battle, Unit caster, object targetInfo);
    }
}
