namespace Mancala.Core.Data;
[SingletonGame]
public class MancalaGameContainer : BasicGameContainer<MancalaPlayerItem, MancalaSaveInfo>
{
    public MancalaGameContainer(BasicData basicData,
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