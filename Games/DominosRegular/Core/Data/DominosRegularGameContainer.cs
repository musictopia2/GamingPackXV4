namespace DominosRegular.Core.Data;
[SingletonGame]
public class DominosRegularGameContainer : BasicGameContainer<DominosRegularPlayerItem, DominosRegularSaveInfo>
{
    public DominosRegularGameContainer(BasicData basicData,
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