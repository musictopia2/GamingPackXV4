namespace ThreeLetterFun.Core.Data;
[SingletonGame]
public class ThreeLetterFunGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random) : BasicGameContainer<ThreeLetterFunPlayerItem, ThreeLetterFunSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
{
}