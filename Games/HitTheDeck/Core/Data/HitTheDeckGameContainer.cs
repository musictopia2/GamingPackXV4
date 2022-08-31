namespace HitTheDeck.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class HitTheDeckGameContainer : CardGameContainer<HitTheDeckCardInformation, HitTheDeckPlayerItem, HitTheDeckSaveInfo>
{
    public HitTheDeckGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<HitTheDeckCardInformation> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
}