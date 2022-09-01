namespace SnagCardGame.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class SnagCardGameGameContainer : TrickGameContainer<SnagCardGameCardInformation, SnagCardGamePlayerItem, SnagCardGameSaveInfo, EnumSuitList>
{
    public SnagCardGameGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<SnagCardGameCardInformation> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
    internal Func<int, Task>? TakeCardAsync { get; set; }
}