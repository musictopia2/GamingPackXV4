namespace ChineseCheckers.Core.Data;
[SingletonGame]
public class ChineseCheckersGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random) : BasicGameContainer<ChineseCheckersPlayerItem, ChineseCheckersSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
{
    public Func<int, Task>? MakeMoveAsync { get; set; }
    public AnimateBasicGameBoard Animates { get; set; } = new AnimateBasicGameBoard(aggregator);
    public ChineseCheckersVMData? Model { get; set; }
}