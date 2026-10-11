namespace BasicMultiplayerDiceGames.Core.Data;
[SingletonGame]
public class BasicMultiplayerDiceGamesGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random) : BasicGameContainer<BasicMultiplayerDiceGamesPlayerItem, BasicMultiplayerDiceGamesSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
{
}