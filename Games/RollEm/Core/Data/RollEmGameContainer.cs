namespace RollEm.Core.Data;
[SingletonGame]
public class RollEmGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random) : BasicGameContainer<RollEmPlayerItem, RollEmSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
{
    public Dictionary<int, NumberInfo> NumberList { get; set; } = [];
    public Func<int, Task>? MakeMoveAsync { get; set; }
}