namespace ConnectTheDots.Core.Data;
[SingletonGame]
[AutoReset]
public class ConnectTheDotsGameContainer : BasicGameContainer<ConnectTheDotsPlayerItem, ConnectTheDotsSaveInfo>
{
    public ConnectTheDotsGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
    {
    }
    public Dictionary<int, SquareInfo>? SquareList { get; set; }
    public Dictionary<int, LineInfo>? LineList { get; set; }
    public Dictionary<int, DotInfo>? DotList { get; set; }
    public LineInfo PreviousLine { get; set; } = new LineInfo();
    public DotInfo PreviousDot { get; set; } = new DotInfo();
    public Func<int, Task>? MakeMoveAsync { get; set; }
}