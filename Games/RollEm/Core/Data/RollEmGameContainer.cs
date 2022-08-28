namespace RollEm.Core.Data;
[SingletonGame]
public class RollEmGameContainer : BasicGameContainer<RollEmPlayerItem, RollEmSaveInfo>
{
    public RollEmGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
    {
    }
    public Dictionary<int, NumberInfo> NumberList { get; set; } = new Dictionary<int, NumberInfo>();
    public Func<int, Task>? MakeMoveAsync { get; set; }
}