namespace Mancala.Core.Data;
[SingletonGame]
public class MancalaGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random) : BasicGameContainer<MancalaPlayerItem, MancalaSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
{
}