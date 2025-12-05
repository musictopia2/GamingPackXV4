namespace MonopolyCardGame.Core.Logic;
public static class ClonedExtensions
{
    extension(MonopolyCardGameCardInformation card)
    {
        public MonopolyCardGameCardInformation GetClonedCard()
    {
        MonopolyCardGameCardInformation output = new();
        output.Populate(card.Deck);
        return output;
    }
    }
    extension (BasicList<MonopolyCardGameCardInformation> cards)
    {
        public BasicList<MonopolyCardGameCardInformation> GetClonedCards()
        {
            BasicList<MonopolyCardGameCardInformation> output = [];
            foreach (var item in cards)
            {
                MonopolyCardGameCardInformation cloned = new();
                cloned.Populate(item.Deck);
                output.Add(cloned);
            }
            return output;
        }
    }
    
}