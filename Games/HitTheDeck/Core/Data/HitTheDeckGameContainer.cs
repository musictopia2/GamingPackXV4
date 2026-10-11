namespace HitTheDeck.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class HitTheDeckGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<HitTheDeckCardInformation> deckList,
    IRandomGenerator random) : CardGameContainer<HitTheDeckCardInformation, HitTheDeckPlayerItem, HitTheDeckSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
}