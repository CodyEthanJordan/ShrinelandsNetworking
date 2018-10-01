using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Networking;
using UnityEngine;

namespace Assets.Scripts.DungeonMaster
{
    public class Unit
    {
        public Guid ID;
        public Guid SideID { get; protected set; }
        public string Name { get; protected set; }

        public Stat HP;
        public Stat Movement;
        public Vector3Int Position = new Vector3Int();

        public event UnitMovedEvent OnUnitMoved;
        public event StatsUpdatedEvent OnStatsChanged;

        public Unit(string name, Guid sideID, Vector3Int pos)
        {
            ID = Guid.NewGuid();
            this.SideID = sideID;
            this.Name = name;
            this.Position = pos;
        }

        public void MoveTo(Vector3Int destination, int movementCost)
        {
            Vector3Int oldPos = Position;
            Position = destination;
            Movement.Current -= movementCost;
            if(OnUnitMoved != null)
            {
                OnUnitMoved(this, ID, oldPos, Position);
            }
            if(OnStatsChanged != null)
            {
                OnStatsChanged(this, this);
            }
        }

        public static Unit GetDefaultDude(string name, Guid sideID, Vector3Int pos)
        {
            var dude = new Unit(name, sideID, pos);
            dude.HP = new Stat(8,8);
            dude.Movement = new Stat(5,5);

            return dude;
        }

        public void HandleResult(Result result)
        {
            switch(result.Type)
            {
                case "Move":
                    Vector3Int oldPos = Position;
                    this.Position = result.UnitAffected.Position;
                    if(OnUnitMoved != null)
                    {
                        OnUnitMoved(this, ID, oldPos, Position);
                    }
                    UpdateStats(result.UnitAffected);
                    break;
            }
        }

        private void UpdateStats(Unit newState)
        {
            this.HP.Max = newState.HP.Max;
            this.HP.Current = newState.HP.Current;
            this.Movement.Max = newState.Movement.Max;
            this.Movement.Current = newState.Movement.Current;
            if(OnStatsChanged != null)
            {
                OnStatsChanged(this, this);
            }
        }

        public Result EndTurn()
        {
            
        }
    }

    public delegate void UnitMovedEvent(object source, Guid ID, Vector3Int oldPos, Vector3Int newPos);
    public delegate void StatsUpdatedEvent(object source, Unit unit);
}
