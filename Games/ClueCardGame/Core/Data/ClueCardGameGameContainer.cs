namespace ClueCardGame.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class ClueCardGameGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<ClueCardGameCardInformation> deckList,
    IRandomGenerator random) : CardGameContainer<ClueCardGameCardInformation, ClueCardGamePlayerItem, ClueCardGameSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
    public PrivateModel? DetectiveDetails { get; set; }
    public ClueCardGameCardInformation GetClonedCard(string name)
    {
        var item = DeckList.Single(x => x.Name == name);
        ClueCardGameCardInformation output = new();
        output.Populate(item.Deck);
        return output; //will clone.
    }
    public bool CanGiveCard(ClueCardGameCardInformation thisCard)
    {
        if (thisCard.Name == SaveRoot!.CurrentPrediction!.FirstName)
        {
            return true;
        }
        if (thisCard.Name == SaveRoot.CurrentPrediction.SecondName)
        {
            return true;
        }
        return false;
    }
    
}