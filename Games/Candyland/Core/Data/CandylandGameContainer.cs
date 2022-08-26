namespace Candyland.Core.Data;
[SingletonGame]
public class CandylandGameContainer : BasicGameContainer<CandylandPlayerItem, CandylandSaveInfo>
{
    public CandylandGameContainer(BasicData basicData,
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