﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.DungeonMaster
{
    public class Deck
    {
        public List<Card> Cards;
        public Card DrawnCard;

        public Deck(int hits, int armors, int dodges)
        {
            Cards = new List<Card>();
            for (int i = 0; i < hits; i++)
            {
                Cards.Add(new Card(Card.CardType.Hit, "you hit"));
            }
            for (int i = 0; i < armors; i++)
            {
                Cards.Add(new Card(Card.CardType.Armor, "armor reduces damage"));
            }
            for (int i = 0; i < dodges; i++)
            {
                Cards.Add(new Card(Card.CardType.Miss, "miss"));
            }
        }

        public Card Draw(int? fated_outcome = null)
        {
            if(fated_outcome.HasValue)
            {
                DrawnCard = Cards[fated_outcome.Value];
                return DrawnCard;
            }
            else
            {
                DrawnCard = Cards[UnityEngine.Random.Range(0, Cards.Count)];
                return DrawnCard;
            }
        }
    }
}
