using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.DungeonMaster
{
    public class Deck
    {
        public static Random rand = new Random();

        public List<Card> Cards;
        public Card DrawnCard;

        public Deck()
        {
            Cards = new List<Card>();
        }

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
            if (fated_outcome.HasValue)
            {
                DrawnCard = Cards[fated_outcome.Value];
                return DrawnCard;
            }
            else
            {
                DrawnCard = Cards[rand.Next(0, Cards.Count)];
                return DrawnCard;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var card in Cards)
            {
                sb.Append(card.ToString() + " ");
            }
            sb.AppendLine();
            return sb.ToString();
        }

        internal void AddCards(Card card, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Cards.Add(card);
            }
        }
    }
}
