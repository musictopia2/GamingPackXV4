namespace RoundsCardGame.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class RoundsCardGameGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<RoundsCardGameCardInformation> deckList,
    IRandomGenerator random) : TrickGameContainer<RoundsCardGameCardInformation, RoundsCardGamePlayerItem, RoundsCardGameSaveInfo, EnumSuitList>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
}