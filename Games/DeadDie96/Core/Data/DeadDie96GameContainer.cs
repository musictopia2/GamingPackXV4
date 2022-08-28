namespace DeadDie96.Core.Data;
[SingletonGame]
public class DeadDie96GameContainer : BasicGameContainer<DeadDie96PlayerItem, DeadDie96SaveInfo>
{
    public DeadDie96GameContainer(BasicData basicData,
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