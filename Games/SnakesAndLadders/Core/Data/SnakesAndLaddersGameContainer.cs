namespace SnakesAndLadders.Core.Data;
[SingletonGame]
public class SnakesAndLaddersGameContainer : BasicGameContainer<SnakesAndLaddersPlayerItem, SnakesAndLaddersSaveInfo>
{
    public SnakesAndLaddersGameContainer(BasicData basicData,
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