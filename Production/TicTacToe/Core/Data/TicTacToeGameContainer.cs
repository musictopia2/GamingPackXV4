namespace TicTacToe.Core.Data;
[SingletonGame]
public class TicTacToeGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random) : BasicGameContainer<TicTacToePlayerItem, TicTacToeSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
{
}