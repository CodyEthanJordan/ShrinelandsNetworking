using Assets.Scripts.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.DungeonMaster
{
    public class Card
    {
        public CardType Type;
        public string Description;

        public Card(CardType type, string description)
        {
            this.Type = type;
            this.Description = description;
        }

        public char GetChar()
        {
            switch(Type)
            {
                case CardType.Hit:
                    return 'H';
                case CardType.Miss:
                    return 'M';
                case CardType.Armor:
                    return 'A';
                default:
                    return ' ';
            }
        }

        public override string ToString()
        {
            return this.GetChar().ToString();
        }

        public enum CardType
        {
            Hit,
            Armor,
            Miss
        }
    }
}
