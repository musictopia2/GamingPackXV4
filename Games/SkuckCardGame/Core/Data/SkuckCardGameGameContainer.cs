namespace SkuckCardGame.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class SkuckCardGameGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<SkuckCardGameCardInformation> deckList,
    IRandomGenerator random) : TrickGameContainer<SkuckCardGameCardInformation, SkuckCardGamePlayerItem, SkuckCardGameSaveInfo, EnumSuitList>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
    public Func<Task>? ComputerTurnAsync { get; set; }
    public Func<Task>? StartNewTrickAsync { get; set; }
    public Func<Task>? ShowHumanCanPlayAsync { get; set; }
}