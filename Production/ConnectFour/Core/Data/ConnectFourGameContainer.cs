namespace ConnectFour.Core.Data;
[SingletonGame]
public class ConnectFourGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random) : BasicGameContainer<ConnectFourPlayerItem, ConnectFourSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
{
}