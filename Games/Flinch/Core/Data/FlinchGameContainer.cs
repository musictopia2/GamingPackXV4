namespace Flinch.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class FlinchGameContainer : CardGameContainer<FlinchCardInformation, FlinchPlayerItem, FlinchSaveInfo>
{
    public FlinchGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<FlinchCardInformation> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
    internal Func<Task>? LoadPlayerPilesAsync { get; set; }
    internal Func<int, int, bool>? IsValidMove { get; set; }
}