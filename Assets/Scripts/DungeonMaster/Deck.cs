using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.DungeonMaster
{
    public class Deck
    {

        public static CardType BasicDraw(int hits, int armors, int dodges, int? fated_outcome)
        {
            var cards = new List<CardType>();
            for (int i = 0; i < hits; i++)
            {
                cards.Add(CardType.Hit);
            }
            for (int i = 0; i < armors; i++)
            {
                cards.Add(CardType.Armor);
            }
            for (int i = 0; i < dodges; i++)
            {
                cards.Add(CardType.Dodge);
            }

            if(fated_outcome.HasValue)
            {
                return cards[fated_outcome.Value];
            }
            else
            {
                int i = UnityEngine.Random.Range(0, cards.Count);
                return cards[i];
            }
        }

        public enum CardType
        {
            Hit, Armor, Dodge
        }
    }
}
