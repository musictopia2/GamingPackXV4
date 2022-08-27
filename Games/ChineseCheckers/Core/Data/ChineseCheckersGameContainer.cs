namespace ChineseCheckers.Core.Data;
[SingletonGame]
public class ChineseCheckersGameContainer : BasicGameContainer<ChineseCheckersPlayerItem, ChineseCheckersSaveInfo>
{
    public ChineseCheckersGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
    {
        Animates = new AnimateBasicGameBoard(aggregator);
    }
    public Func<int, Task>? MakeMoveAsync { get; set; }
    public AnimateBasicGameBoard Animates { get; set; }
    public ChineseCheckersVMData? Model { get; set; }
}