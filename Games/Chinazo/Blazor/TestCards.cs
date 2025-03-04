namespace Chinazo.Blazor;
public class TestCards : ITestCardSetUp<ChinazoCard, ChinazoPlayerItem>
{
    Task ITestCardSetUp<ChinazoCard, ChinazoPlayerItem>.SetUpTestHandsAsync(PlayerCollection<ChinazoPlayerItem> playerList, IListShuffler<ChinazoCard> deckList)
    {
        var self = playerList.GetSelf();
        var firsts = deckList.Where(x => x.Value == EnumRegularCardValueList.Three).Take(3);
        self.StartUpList.AddRange(firsts);
        for (int i = 11; i < 15; i++)
        {
            var nexts = deckList.Where(x => x.Value.Value == i && x.Suit == EnumSuitList.Spades).Take(1);
            self.StartUpList.AddRange(nexts);
        }
        firsts = deckList.Where(x => x.Value == EnumRegularCardValueList.Joker).Take(1);
        self.StartUpList.AddRange(firsts);
        return Task.CompletedTask;
    }
}
