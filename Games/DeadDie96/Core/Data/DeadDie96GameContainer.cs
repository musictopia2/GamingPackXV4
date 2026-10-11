namespace DeadDie96.Core.Data;
[SingletonGame]
public class DeadDie96GameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random) : BasicGameContainer<DeadDie96PlayerItem, DeadDie96SaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
{
}