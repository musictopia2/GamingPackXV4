namespace BattleshipLite.Core.Data;
[SingletonGame]
public class BattleshipLiteGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random) : BasicGameContainer<BattleshipLitePlayerItem, BattleshipLiteSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
{
}