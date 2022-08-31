namespace Skipbo.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class SkipboGameContainer : CardGameContainer<SkipboCardInformation, SkipboPlayerItem, SkipboSaveInfo>
{
    public SkipboGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<SkipboCardInformation> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
    internal Func<Task>? LoadPlayerPilesAsync { get; set; }
    internal Func<int, int, bool>? IsValidMove { get; set; }
}