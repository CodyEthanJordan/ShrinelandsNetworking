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
        public ResultType Type;
        public string Title;
        public string Description;
        public List<Effect> Effects;

        public Result(ResultType type, string title, string description, Effect effect)
        {
            this.Type = type;
            this.Title = title;
            this.Description = description;
            Effects = new List<Effect>();
            Effects.Add(effect);
        }

        public enum ResultType
        {
            Movement,
            GenericUpdate
        }
    }

    public class Effect
    {
        // TODO: deck
        public Unit NewStatus;

        public Effect(Unit newStatus)
        {
            this.NewStatus = newStatus;
        }
    }
}
