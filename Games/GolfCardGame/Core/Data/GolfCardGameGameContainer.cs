namespace GolfCardGame.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class GolfCardGameGameContainer : CardGameContainer<RegularSimpleCard, GolfCardGamePlayerItem, GolfCardGameSaveInfo>
{
    public GolfCardGameGameContainer(BasicData basicData,
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
    internal Func<int, Task>? ChangeHandAsync { get; set; }
    internal Func<int, Task>? ChangeUnknownAsync { get; set; }
}