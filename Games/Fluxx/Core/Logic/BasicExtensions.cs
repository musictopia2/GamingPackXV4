namespace Fluxx.Core.Logic;
public static class BasicExtensions
{
    public static void ScrambleKeepers(this PlayerCollection<FluxxPlayerItem> playerList)
    {
        var thisList = playerList.Where(items => items.KeeperList.Count > 0).Select(items => items.KeeperList.Count).ToBasicList();
        if (thisList.Count < 2)
        {
            throw new CustomBasicException("Cannot scramble the keepers");
        }
        var firstKeeperTempList = playerList.Select(items => items.KeeperList).ToBasicList();
        DeckRegularDict<KeeperCard> output = new();
        firstKeeperTempList.ForEach(thisItem => output.AddRange(thisItem));
        output.ShuffleList();
        if (output.Count != thisList.Sum(items => items))
        {
            throw new CustomBasicException("Numbers don't match");
        }
        playerList.ForEach(thisPlayer =>
        {
            int nums = thisList[thisPlayer.Id - 1]; //because 0 based
            thisPlayer.KeeperList.ReplaceRange(output.Take(nums));
            if (thisPlayer.KeeperList.Count != thisList[thisPlayer.Id - 1])
            {
                throw new CustomBasicException("Numbers don't match");
            }
            output = output.Skip(nums).ToRegularDeckDict();
        });
    }
    public static DeckRegularDict<FluxxCardInformation> GetFluxxCardListFromDeck(this BasicList<int> cardList, FluxxGameContainer gameContainer)
    {
        DeckRegularDict<FluxxCardInformation> output = new();
        cardList.ForEach(thisDeck =>
        {
            output.Add(gameContainer.DeckList!.GetSpecificItem(thisDeck));
        });
        return output;
    }
    public static int RulesThatCanBeDiscarded<T>(this BasicList<T> ruleList)
    {
        int counts = ruleList.Count;
        if (counts <= 1)
        {
            return 1;
        }
        return counts / 2;
    }
}