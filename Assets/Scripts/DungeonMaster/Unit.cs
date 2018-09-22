﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.Scripts.DungeonMaster
{
    public class Unit
    {
        public Guid ID { get; protected set; }
        public Guid SideID { get; protected set; }
        public string Name { get; protected set; }

        public int HP;
        public int Movement;
        public Vector3Int Position = new Vector3Int();

        public Unit(string name, Guid sideID, Vector3Int pos)
        {
            ID = Guid.NewGuid();
            this.SideID = sideID;
            this.Name = name;
            this.Position = pos;
        }

        public static Unit GetDefaultDude(string name, Guid sideID, Vector3Int pos)
        {
            var dude = new Unit(name, sideID, pos);
            dude.HP = 8;
            dude.Movement = 5;

            return dude;
        }
    }
}