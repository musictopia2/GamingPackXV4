namespace Skipbo.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class SkipboGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<SkipboCardInformation> deckList,
    IRandomGenerator random) : CardGameContainer<SkipboCardInformation, SkipboPlayerItem, SkipboSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
    internal Action? LoadPlayerPiles { get; set; }
    internal Func<int, int, bool>? IsValidMove { get; set; }
}