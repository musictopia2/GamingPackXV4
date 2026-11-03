namespace GalaxyCardGame.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class GalaxyCardGameGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<GalaxyCardGameCardInformation> deckList,
    IRandomGenerator random) : TrickGameContainer<GalaxyCardGameCardInformation, GalaxyCardGamePlayerItem, GalaxyCardGameSaveInfo, EnumSuitList>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
}