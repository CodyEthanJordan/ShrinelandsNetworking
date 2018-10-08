﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Networking;
using UnityEngine;

namespace Assets.Scripts.DungeonMaster.Abilities
{
    public class Attack : Ability
    {
        public Attack()
        {
            Name = "attack";
            Description = "basic attack on adjacent square";
        }

        public override bool CanBeUsed(Battle battle, Unit caster)
        {
            return caster.Stamina.Current > 0 && !caster.HasActed;
        }

        public override List<Result> UseAbility(Battle battle, Unit caster, object targetInfo)
        {
            Vector3Int dir = (Vector3Int)targetInfo;
            var results = new List<Result>();

            if (dir.magnitude >= 2)
            {
                results.Add(new Result(Result.ResultType.InvalidAction, "too far", "target too far away", null));
                return results;
            }

            caster.HasActed = true;

            var hitUnit = battle.units.FirstOrDefault(u => u.Position == caster.Position + dir);
            if (hitUnit == null)
            {
                results.Add(new Result(Result.ResultType.Generic, "nothing there", "you swing at nothing", null));
                return results;
            }

            Deck deck = new Deck(caster.Expertise.Current, 0, hitUnit.Stamina.Current);
            Card outcome = deck.Draw();
            Result result;
            switch (outcome.Type)
            {
                case Card.CardType.Hit:
                     result = new Result(Result.ResultType.Deck, "hit",
                        hitUnit.Name + " takes " + caster.Strength.Current + " damage from " + caster.Name, null);
                    result.OutcomeDeck = deck;
                    hitUnit.TakeDamage(caster.Strength.Current);
                    results.Add(result);
                    return results;
                case Card.CardType.Miss:
                     result = new Result(Result.ResultType.Deck, "hit",
                        hitUnit.Name + " dodges", null);
                    result.OutcomeDeck = deck;
                    results.Add(result);
                    return results;
            }

            // TODO: log, should never get here
            return null;

        }
    }
}
