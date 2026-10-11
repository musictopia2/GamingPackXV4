namespace Bingo.Core.Data;
[SingletonGame]
public class BingoGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random) : BasicGameContainer<BingoPlayerItem, BingoSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
{
}