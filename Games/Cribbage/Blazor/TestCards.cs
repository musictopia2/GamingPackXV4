namespace Cribbage.Blazor;
public class TestCards : ITestCardSetUp<CribbageCard, CribbagePlayerItem>
{
    Task ITestCardSetUp<CribbageCard, CribbagePlayerItem>.SetUpTestHandsAsync(PlayerCollection<CribbagePlayerItem> playerList, IListShuffler<CribbageCard> deckList)
    {
        //player 1 has to have 2 and 4
        //player 2 has to have 3 and 5

        var p11 = deckList.Where(x => x.Value == EnumRegularCardValueList.Two).Take(1);
        var p12 = deckList.Where(x => x.Value == EnumRegularCardValueList.Four).Take(1);
        var p21 = deckList.Where(x => x.Value == EnumRegularCardValueList.Three).Take(1);
        var p22 = deckList.Where(x => x.Value == EnumRegularCardValueList.Five).Take(1);
        var p13 = deckList.Where(x => x.Value == EnumRegularCardValueList.Six).Take(1);
        var p23 = deckList.Where(x => x.Value == EnumRegularCardValueList.Seven).Take(1);
        playerList.First().StartUpList.AddRange(p11);
        playerList.First().StartUpList.AddRange(p12);
        playerList.Last().StartUpList.AddRange(p21);
        playerList.Last().StartUpList.AddRange(p22);
        playerList.First().StartUpList.AddRange(p13);
        playerList.Last().StartUpList.AddRange(p23);
        //var list = deckList.Where(x => x.Suit == EnumSuitList.Hearts).Take(4);
        //var list = deckList.Where(x => x.Value == EnumRegularCardValueList.Three);
        //var firsts = list.Take(6);
        //var seconds = list.Skip(6);
        //playerList.First().StartUpList.AddRange(list);
        //playerList.Last().StartUpList.AddRange(seconds);
        return Task.CompletedTask;
    }
}