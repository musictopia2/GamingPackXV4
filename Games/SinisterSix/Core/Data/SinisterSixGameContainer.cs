namespace SinisterSix.Core.Data;
[SingletonGame]
public class SinisterSixGameContainer : BasicGameContainer<SinisterSixPlayerItem, SinisterSixSaveInfo>
{
    public SinisterSixGameContainer(BasicData basicData,
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