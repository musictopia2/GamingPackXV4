namespace MonopolyDicedGame.Core.Data;
[SingletonGame]
public class MonopolyDicedGameGameContainer : BasicGameContainer<MonopolyDicedGamePlayerItem, MonopolyDicedGameSaveInfo>
{
    public MonopolyDicedGameGameContainer(BasicData basicData,
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