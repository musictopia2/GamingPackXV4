namespace A21DiceGame.Core.Data;
[SingletonGame]
public class A21DiceGameGameContainer : BasicGameContainer<A21DiceGamePlayerItem, A21DiceGameSaveInfo>
{
    public A21DiceGameGameContainer(BasicData basicData,
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