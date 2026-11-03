namespace GolfCardGame.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class GolfCardGameGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<RegularSimpleCard> deckList,
    IRandomGenerator random) : CardGameContainer<RegularSimpleCard, GolfCardGamePlayerItem, GolfCardGameSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
    internal Func<int, Task>? ChangeHandAsync { get; set; }
    internal Func<int, Task>? ChangeUnknownAsync { get; set; }
}