namespace PickelCardGame.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class PickelCardGameGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<PickelCardGameCardInformation> deckList,
    IRandomGenerator random) : TrickGameContainer<PickelCardGameCardInformation, PickelCardGamePlayerItem, PickelCardGameSaveInfo, EnumSuitList>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
    internal Func<Task>? StartNewTrickAsync { get; set; }
}