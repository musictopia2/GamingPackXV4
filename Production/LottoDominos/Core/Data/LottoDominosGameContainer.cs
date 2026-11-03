namespace LottoDominos.Core.Data;
[SingletonGame]
public class LottoDominosGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random) : BasicGameContainer<LottoDominosPlayerItem, LottoDominosSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
{
}