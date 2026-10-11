namespace Racko.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class RackoGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<RackoCardInformation> deckList,
    IRandomGenerator random) : CardGameContainer<RackoCardInformation, RackoPlayerItem, RackoSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
}