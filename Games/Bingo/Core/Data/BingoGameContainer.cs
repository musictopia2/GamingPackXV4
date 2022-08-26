namespace Bingo.Core.Data;
[SingletonGame]
public class BingoGameContainer : BasicGameContainer<BingoPlayerItem, BingoSaveInfo>
{
    public BingoGameContainer(BasicData basicData,
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