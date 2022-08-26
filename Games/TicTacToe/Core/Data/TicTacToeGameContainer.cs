namespace TicTacToe.Core.Data;
[SingletonGame]
public class TicTacToeGameContainer : BasicGameContainer<TicTacToePlayerItem, TicTacToeSaveInfo>
{
    public TicTacToeGameContainer(BasicData basicData,
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