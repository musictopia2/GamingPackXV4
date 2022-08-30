namespace Cribbage.Blazor;
public class TestCards : ITestCardSetUp<CribbageCard, CribbagePlayerItem>
{
    Task ITestCardSetUp<CribbageCard, CribbagePlayerItem>.SetUpTestHandsAsync(PlayerCollection<CribbagePlayerItem> playerList, IListShuffler<CribbageCard> deckList)
    {
        var list = deckList.Where(x => x.Suit == EnumSuitList.Hearts).Take(4);
        //var list = deckList.Where(x => x.Value == EnumRegularCardValueList.Three);
        //var firsts = list.Take(6);
        //var seconds = list.Skip(6);
        playerList.First().StartUpList.AddRange(list);
        //playerList.Last().StartUpList.AddRange(seconds);
        return Task.CompletedTask;
    }
}