namespace TileRummy.Core.Data;
[SingletonGame]
public class TileRummyGameContainer : BasicGameContainer<TileRummyPlayerItem, TileRummySaveInfo>
{
    public TileRummyGameContainer(BasicData basicData,
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