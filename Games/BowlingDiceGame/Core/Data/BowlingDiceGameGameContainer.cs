namespace BowlingDiceGame.Core.Data;
[SingletonGame]
public class BowlingDiceGameGameContainer : BasicGameContainer<BowlingDiceGamePlayerItem, BowlingDiceGameSaveInfo>
{
    public BowlingDiceGameGameContainer(BasicData basicData,
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