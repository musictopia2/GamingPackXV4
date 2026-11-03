namespace TileRummy.Core.Data;
[SingletonGame]
public class TileRummyGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random) : BasicGameContainer<TileRummyPlayerItem, TileRummySaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
{
}