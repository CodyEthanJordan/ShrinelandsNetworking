﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.DungeonMaster.Abilities;
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
        public Stat Stamina;
        public Stat Expertise;
        public Stat Strength;
        public Vector3Int Position = new Vector3Int();
        public bool HasActed;
        public bool SlimeImmune;

        public List<Ability> Abilities;

        public event UnitMovedEvent OnUnitMoved;
        public event StatsUpdatedEvent OnStatsChanged;

        public bool IsDead { get { return HP.Current <= 0; } }

        public Unit(string name, Guid sideID, Vector3Int pos)
        {
            ID = Guid.NewGuid();
            this.SideID = sideID;
            this.Name = name;
            this.Position = pos;
            Abilities = new List<Ability>();
            HasActed = false;
        }

        public void MoveTo(Vector3Int destination, int movementCost)
        {
            if (movementCost > this.Movement.Current)
            {
                return;
            }
            Vector3Int oldPos = Position;
            Position = destination;
            Movement.Current -= movementCost;

            if (OnUnitMoved != null)
            {
                OnUnitMoved(this, ID, oldPos, Position);
            }
            if (OnStatsChanged != null)
            {
                OnStatsChanged(this, this);
            }
        }

        public static Unit GetDefaultDude(string name, Guid sideID, Vector3Int pos)
        {
            var dude = new Unit(name, sideID, pos);
            dude.HP = new Stat(8, 8);
            dude.Movement = new Stat(5, 5);
            dude.Stamina = new Stat(5, 5);
            dude.Expertise = new Stat(4, 4);
            dude.Strength = new Stat(4, 4);
            dude.Abilities.Add(new Attack());
            return dude;
        }

        internal void TakeDamage(int dmg)
        {
            //TODO: check damage type, make sure HP doesn't fall below 0, fire events
            HP.Current -= dmg;
        }

        public void HandleResult(Result result)
        {
            switch (result.Type)
            {
            }
        }

        private void UpdateStats(Unit newState)
        {
            this.HP.Max = newState.HP.Max;
            this.HP.Current = newState.HP.Current;
            this.Movement.Max = newState.Movement.Max;
            this.Movement.Current = newState.Movement.Current;
            if (OnStatsChanged != null)
            {
                OnStatsChanged(this, this);
            }
        }

        public List<Result> EndTurn()
        {
            return new List<Result>(); //TODO: anything happen? 
        }

        public List<Result> StartTurn(Battle b)
        {
            Stamina.Current += 2;
            HasActed = false;
            Movement.Current = Movement.Max;

            var results = new List<Result>();
            results.Add(new Result(Result.ResultType.Generic, "new turn", this.Name + " is ready for battle!", null));

            //TODO: make block delegate?
            var blockIn = b.map.BlockAt(this.Position);
            results.AddRange(blockIn.ApplyStartTurnEffects(b, this));

            return results;
        }

        public string ShowInfo(Battle b)
        {
            var sb = new StringBuilder();
            var side = b.sides.First(s => s.ID == this.SideID);
            sb.AppendLine(Name + "   (" + side.Name + ")");
            sb.AppendLine(Position.ToString());
            sb.AppendLine("HP: " + HP.ToString());
            sb.AppendLine("Movement: " + Movement.ToString());
            sb.AppendLine("Stamina: " + Stamina.ToString());
            sb.AppendLine("Strength: " + Strength.ToString());
            sb.AppendLine("Expertise: " + Expertise.ToString());

            sb.Append("Abilities: ");
            foreach (var ability in Abilities)
            {
                sb.Append(ability.Name + ",");
            }
            sb.AppendLine("");

            sb.AppendLine("Standing on " + b.map.StandingOn(this).Name);

            var blockIn = b.map.BlockAt(this.Position);
            if (blockIn.Name != "air")
            {
                sb.AppendLine("Inside " + blockIn.Name);
            }

            return sb.ToString();
        }

        internal List<Result> MoveDirection(Battle b, Map.Direction direction)
        {
            var results = new List<Result>();
            if (direction == Map.Direction.Up) //only swimming
            {
                var blockIn = b.map.BlockAt(this.Position);
                var blockAbove = b.map.BlockAt(this.Position + new Vector3Int(0, 0, 1));
                if (blockIn.Flugen && blockAbove.Flugen)
                {
                    MoveTo(this.Position + new Vector3Int(0, 0, 1), 1);
                    return results;
                }
            }
            else
            {
                var blockIn = b.map.BlockAt(this.Position);
                var blockOn = b.map.StandingOn(this);
                var destinationBlock = b.map.BlockAt(this.Position + Map.VectorFromDirection(direction));
                if (destinationBlock.Solid)
                {
                    //check for climbing
                    var blockAbove = b.map.BlockAt(this.Position + Map.VectorFromDirection(direction) +
                        new Vector3Int(0, 0, 1));
                    if (blockAbove.Solid)
                    {
                        results.Add(new Result(Result.ResultType.InvalidAction, "blocked", "cannot move there", null));
                    }
                    else
                    {
                        MoveTo(this.Position + Map.VectorFromDirection(direction) +
                        new Vector3Int(0, 0, 1), blockOn.MoveCost + 1);
                        return results;
                    }
                }

                MoveTo(this.Position + Map.VectorFromDirection(direction), blockOn.MoveCost);
                //check for falling
                var newBlockOn = b.map.BlockAt(this.Position + new Vector3Int(0, 0, -1));
                if (!newBlockOn.Solid)
                {
                    results.AddRange(Fall(b, 0));
                }
                return results;
            }

            throw new NotImplementedException();
        }

        private List<Result> Fall(Battle b, int fallen)
        {
            var results = new List<Result>();
            this.Position = this.Position + new Vector3Int(0, 0, -1);
            var newBlockOn = b.map.BlockAt(this.Position + new Vector3Int(0, 0, -1));
            if (!newBlockOn.Solid)
            {
                results.AddRange(Fall(b, fallen+1));
                return results;
            }
            else
            {
                TakeDamage(fallen);
                results.Add(new Result(Result.ResultType.Generic, "fall", Name + " falls and takes " + fallen + " damage", null));
                return results;
            }
        }
    }

    public delegate void UnitMovedEvent(object source, Guid ID, Vector3Int oldPos, Vector3Int newPos);
    public delegate void StatsUpdatedEvent(object source, Unit unit);
}
