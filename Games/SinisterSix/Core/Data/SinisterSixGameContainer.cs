namespace SinisterSix.Core.Data;
[SingletonGame]
public class SinisterSixGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random) : BasicGameContainer<SinisterSixPlayerItem, SinisterSixSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
{
}