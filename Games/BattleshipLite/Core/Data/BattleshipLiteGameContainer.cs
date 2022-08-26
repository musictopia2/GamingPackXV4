namespace BattleshipLite.Core.Data;
[SingletonGame]
public class BattleshipLiteGameContainer : BasicGameContainer<BattleshipLitePlayerItem, BattleshipLiteSaveInfo>
{
    public BattleshipLiteGameContainer(BasicData basicData,
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