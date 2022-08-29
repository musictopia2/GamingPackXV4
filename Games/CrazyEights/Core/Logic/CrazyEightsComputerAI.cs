namespace CrazyEights.Core.Logic;
public static class CrazyEightsComputerAI
{
    public static int CardToPlay(CrazyEightsSaveInfo saveroot)
    {
        CrazyEightsPlayerItem player = saveroot.PlayerList.GetWhoPlayer();
        DeckRegularDict<RegularSimpleCard> list = player.MainHandList.Where(x => x.Value != EnumRegularCardValueList.Eight && x.Suit == saveroot.CurrentSuit).ToRegularDeckDict();
        if (list.Count != 0)
        {
            return list.GetRandomItem().Deck;
        }
        list = player.MainHandList.Where(x => x.Value != EnumRegularCardValueList.Eight && x.Value == saveroot.CurrentNumber).ToRegularDeckDict();
        if (list.Count == 1)
        {
            return list.Single().Deck;
        }
        if (list.Count != 0)
        {
            return FindBestCard(list);
        }
        list = player.MainHandList.Where(x => x.Value == EnumRegularCardValueList.Eight).ToRegularDeckDict();
        if (list.Count != 0)
        {
            return list.GetRandomItem().Deck;
        }
        return 0;//0 means needs to draw.
    }
    private static int FindBestCard(DeckRegularDict<RegularSimpleCard> list)
    {
        var bestsuit = list.GroupBy(x => x.Suit).OrderByDescending(Temps => Temps.Count());
        var suituse = bestsuit.First().Key;
        list.KeepConditionalItems(Items => Items.Suit == suituse);
        return list.GetRandomItem().Deck;
    }
    public static EnumSuitList SuitToChoose(CrazyEightsPlayerItem player)
    {
        var best = player.MainHandList.GroupBy(x => x.Suit).OrderByDescending(Temps => Temps.Count());
        return best.First().Key;
    }
}