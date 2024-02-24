namespace MonopolyCardGame.Core.Logic;
public static class ClonedExtensions
{
    public static MonopolyCardGameCardInformation GetClonedCard(this MonopolyCardGameCardInformation card)
    {
        MonopolyCardGameCardInformation output = new();
        output.Populate(card.Deck);
        return output;
    }
    public static BasicList<MonopolyCardGameCardInformation> GetClonedCards(this BasicList<MonopolyCardGameCardInformation> list)
    {
        BasicList<MonopolyCardGameCardInformation> output = [];
        foreach (var item in list)
        {
            MonopolyCardGameCardInformation cloned = new();
            cloned.Populate(item.Deck);
            output.Add(cloned);
        }
        return output;
    }
}