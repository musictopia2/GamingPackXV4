namespace SnakesAndLadders.Core.Data;
[SingletonGame]
public class SnakesAndLaddersGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random) : BasicGameContainer<SnakesAndLaddersPlayerItem, SnakesAndLaddersSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
{
}