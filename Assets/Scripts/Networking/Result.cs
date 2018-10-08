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
        public List<Update> Effects;
        public Deck OutcomeDeck;


        public Result(ResultType type, string title, string description, Update effect)
        {
            this.Type = type;
            this.Title = title;
            this.Description = description;
            Effects = new List<Update>();
            Effects.Add(effect);
        }

        public enum ResultType
        {
            Generic,
            Deck,
            InvalidAction,
                
        }
    }

    public class Update
    {
        // TODO: deck
        public Unit NewStatus;

        public Update(Unit newStatus)
        {
            this.NewStatus = newStatus;
        }
    }
}
