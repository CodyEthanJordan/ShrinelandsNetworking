using Assets.Scripts.DungeonMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Networking
{
    public class Result
    {
        public string Type;
        public Unit UnitAffected;
        public Vector3Int Target;

        public Result(string type, Unit unitAffected, Vector3Int target)
        {
            this.Type = type;
            this.UnitAffected = unitAffected;
            this.Target = target;
        }
    }
}
