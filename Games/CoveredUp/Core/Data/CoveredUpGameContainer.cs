namespace CoveredUp.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class CoveredUpGameContainer : CardGameContainer<RegularSimpleCard, CoveredUpPlayerItem, CoveredUpSaveInfo>
{
    public CoveredUpGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<RegularSimpleCard> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
    internal Func<RegularSimpleCard, Task>? PileClickedAsync { get; set; } //this means you clicked the card
}