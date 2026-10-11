namespace BasicMultiplayerGames.Core.Data;
[SingletonGame]
public class BasicMultiplayerGamesGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random) : BasicGameContainer<BasicMultiplayerGamesPlayerItem, BasicMultiplayerGamesSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
{
}