namespace DiceBoardGamesMultiplayer.Core.Data;
[SingletonGame]
public class DiceBoardGamesMultiplayerGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random) : BasicGameContainer<DiceBoardGamesMultiplayerPlayerItem, DiceBoardGamesMultiplayerSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
{
}