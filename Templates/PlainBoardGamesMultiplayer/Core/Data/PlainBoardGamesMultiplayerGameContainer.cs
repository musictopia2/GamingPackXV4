namespace PlainBoardGamesMultiplayer.Core.Data;
[SingletonGame]
public class PlainBoardGamesMultiplayerGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random) : BasicGameContainer<PlainBoardGamesMultiplayerPlayerItem, PlainBoardGamesMultiplayerSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
{
}