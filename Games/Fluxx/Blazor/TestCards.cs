namespace Fluxx.Blazor;
public class TestCards : ITestCardSetUp<FluxxCardInformation, FluxxPlayerItem>
{
    Task ITestCardSetUp<FluxxCardInformation, FluxxPlayerItem>.SetUpTestHandsAsync(PlayerCollection<FluxxPlayerItem> playerList, IListShuffler<FluxxCardInformation> deckList)
    {
        var card = deckList.Where(x => x.Deck == 80).Single();
        var player = playerList.GetSelf();
        player.StartUpList.Add(card);
        return Task.CompletedTask;
    }
}