namespace Rook.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class RookGameContainer : TrickGameContainer<RookCardInformation, RookPlayerItem, RookSaveInfo, EnumColorTypes>
{
    public RookGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<RookCardInformation> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
    internal bool ShowedOnce { get; set; } = false;
    internal Func<Task>? StartNewTrickAsync { get; set; }
    internal Action? AfterBidding { get; set; }
    internal Action? StartingStatus { get; set; }
}