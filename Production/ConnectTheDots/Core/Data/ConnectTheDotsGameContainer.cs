namespace ConnectTheDots.Core.Data;
[SingletonGame]
[AutoReset]
public class ConnectTheDotsGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random) : BasicGameContainer<ConnectTheDotsPlayerItem, ConnectTheDotsSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
{
    public Dictionary<int, SquareInfo>? SquareList { get; set; }
    public Dictionary<int, LineInfo>? LineList { get; set; }
    public Dictionary<int, DotInfo>? DotList { get; set; }
    public LineInfo PreviousLine { get; set; } = new ();
    public DotInfo PreviousDot { get; set; } = new ();
    public Func<int, Task>? MakeMoveAsync { get; set; }
}