namespace CrazyEights.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class CrazyEightsGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<RegularSimpleCard> deckList,
    IRandomGenerator random) : CardGameContainer<RegularSimpleCard, CrazyEightsPlayerItem, CrazyEightsSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
}