namespace YachtRace.Core.Data;
[SingletonGame]
public class YachtRaceGameContainer : BasicGameContainer<YachtRacePlayerItem, YachtRaceSaveInfo>
{
    public YachtRaceGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
    {
    }
}