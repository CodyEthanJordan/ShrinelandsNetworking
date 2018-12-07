using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Networking;
using UnityEngine;

namespace Assets.Scripts.DungeonMaster
{
    public abstract class Buff
    {
        public string Name;


        public abstract List<Result> UnitMoved(Battle b, Unit unit, Vector3Int oldPos, Vector3Int position);

        public abstract List<Result> UnitUsedAbility(Battle battle, Unit unit, Ability ability);
    }

    // buff ideas
    // slime trail - leave slime behind when move, can cast it on foes
    // strike from shadows, +2 to hit on melee attacks, goes away after taking any action or moving
    // slimewalkers - don't take damage from slime, slime movement faster
}
