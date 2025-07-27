namespace FourSuitRummy.Core.Cards;
[SingletonGame]
public class FourSuitRummyStartJokerCards : ICardPreDealSetup<RegularRummyCard, FourSuitRummyPlayerItem>
{
    void ICardPreDealSetup<RegularRummyCard, FourSuitRummyPlayerItem>.SetUpPreDealCards(PlayerCollection<FourSuitRummyPlayerItem> playerList, IListShuffler<RegularRummyCard> deckList)
    {
        var list = deckList.Where(x => x.Value == EnumRegularCardValueList.Joker).ToRegularDeckDict();
        if (list.Count < playerList.Count)
        {
            throw new CustomBasicException("Not enough Jokers to assign one to each player.");
        }
        foreach (var item in playerList)
        {
            var joker = list.First();
            item.StartUpList.Add(joker);
            list.RemoveFirstItem();
        }
    }
}