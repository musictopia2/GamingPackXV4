namespace Battleship.Core.Data;
[SingletonGame]
public class BattleshipGameContainer : BasicGameContainer<BattleshipPlayerItem, BattleshipSaveInfo>
{
    public BattleshipGameContainer(BasicData basicData,
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