namespace MonopolyDicedGame.Core.Data;
[SingletonGame]
public class MonopolyDicedGameGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random) : BasicGameContainer<MonopolyDicedGamePlayerItem, MonopolyDicedGameSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
{
    public Func<int, Task>? SelectOneMainAsync { get; set; }
}