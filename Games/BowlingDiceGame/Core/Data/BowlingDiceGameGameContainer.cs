namespace BowlingDiceGame.Core.Data;
[SingletonGame]
public class BowlingDiceGameGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random) : BasicGameContainer<BowlingDiceGamePlayerItem, BowlingDiceGameSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
{
}