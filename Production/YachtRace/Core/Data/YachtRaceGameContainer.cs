namespace YachtRace.Core.Data;
[SingletonGame]
public class YachtRaceGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random) : BasicGameContainer<YachtRacePlayerItem, YachtRaceSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
{
}