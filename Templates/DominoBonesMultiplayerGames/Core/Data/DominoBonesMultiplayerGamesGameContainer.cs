namespace DominoBonesMultiplayerGames.Core.Data;
[SingletonGame]
public class DominoBonesMultiplayerGamesGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random) : BasicGameContainer<DominoBonesMultiplayerGamesPlayerItem, DominoBonesMultiplayerGamesSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
{
}