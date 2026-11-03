namespace CoveredUp.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class CoveredUpGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<RegularSimpleCard> deckList,
    IRandomGenerator random) : CardGameContainer<RegularSimpleCard, CoveredUpPlayerItem, CoveredUpSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
    internal Func<RegularSimpleCard, Task>? PileClickedAsync { get; set; } //this means you clicked the card
}