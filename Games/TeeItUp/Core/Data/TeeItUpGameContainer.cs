namespace TeeItUp.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class TeeItUpGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<TeeItUpCardInformation> deckList,
    IRandomGenerator random) : CardGameContainer<TeeItUpCardInformation, TeeItUpPlayerItem, TeeItUpSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
    internal Func<TeeItUpPlayerItem, TeeItUpCardInformation, Task>? BoardClickedAsync { get; set; }
}