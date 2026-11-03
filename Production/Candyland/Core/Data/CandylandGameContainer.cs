namespace Candyland.Core.Data;
[SingletonGame]
public class CandylandGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random) : BasicGameContainer<CandylandPlayerItem, CandylandSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
{
}