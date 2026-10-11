namespace Battleship.Core.Data;
[SingletonGame]
public class BattleshipGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random) : BasicGameContainer<BattleshipPlayerItem, BattleshipSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
{
}