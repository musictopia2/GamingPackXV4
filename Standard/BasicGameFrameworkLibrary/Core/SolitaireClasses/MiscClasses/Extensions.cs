namespace BasicGameFrameworkLibrary.Core.SolitaireClasses.MiscClasses;
public static class Extensions
{
    extension(IDeckDict<SolitaireCard> list)
    {
        public DeckRegularDict<SolitaireCard> ListValidCardsSameSuit()
        {
            int x;
            int previousNumber = 0;
            DeckRegularDict<SolitaireCard> output = new();
            EnumSuitList previousSuit = EnumSuitList.None;
            for (x = list.Count; x >= 1; x += -1)
            {
                var thisCard = list[x - 1];
                if (x == list.Count)
                {
                    previousSuit = thisCard.Suit;
                    previousNumber = thisCard.Value.Value;
                    output.Add(thisCard);
                }
                else
                {
                    if (previousNumber + 1 == thisCard.Value.Value && thisCard.Suit == previousSuit)
                    {
                        output.Add(thisCard);
                    }
                    else
                    {
                        return output;
                    }
                    previousNumber = thisCard.Value.Value;
                }
            }
            return output;
        }
        public DeckRegularDict<SolitaireCard> ListValidCardsAlternateColors()
        {
            int x;
            int previousNumber = 0;
            DeckRegularDict<SolitaireCard> output = new();
            EnumRegularColorList previousColor = EnumRegularColorList.None;
            for (x = list.Count; x >= 1; x += -1)
            {
                var thisCard = list[x - 1];
                if (x == list.Count)
                {
                    previousColor = thisCard.Color;
                    previousNumber = thisCard.Value.Value;
                    output.Add(thisCard);
                }
                else
                {
                    if (previousNumber + 1 == thisCard.Value.Value && thisCard.Color != previousColor)
                    {
                        output.Add(thisCard);
                    }
                    else
                    {
                        return output;
                    }
                    previousNumber = thisCard.Value.Value;
                    previousColor = thisCard.Color;
                }
            }
            return output;
        }
        public bool CanMoveCardsAlternateColors(SolitaireCard oldCard, ref int lastOne)
        {
            lastOne = -1;
            int x;
            for (x = list.Count; x >= 1; x += -1)
            {
                var newCard = list[x - 1];
                if (newCard.Value.Value + 1 == oldCard.Value.Value && newCard.Color != oldCard.Color)
                {
                    lastOne = x - 1;
                    return true;
                }
            }
            return false;
        }
        public bool CanMoveCardsSameColor(SolitaireCard oldCard, ref int lastOne)
        {
            lastOne = -1;
            int x;
            for (x = list.Count; x >= 1; x += -1)
            {
                var newCard = list[x - 1];
                if (newCard.Value.Value + 1 == oldCard.Value.Value && newCard.Color == oldCard.Color)
                {
                    lastOne = x - 1;
                    return true;
                }
            }
            return false;
        }
        public bool CanMoveCardsRegardlessOfColorOrSuit(SolitaireCard oldCard, ref int lastOne)
        {
            lastOne = -1;
            int x;
            for (x = list.Count; x >= 1; x += -1)
            {
                var newCard = list[x - 1];
                if (newCard.Value.Value + 1 == oldCard.Value.Value)
                {
                    lastOne = x - 1;
                    return true;
                }
            }
            return false;
        }
    }   
}