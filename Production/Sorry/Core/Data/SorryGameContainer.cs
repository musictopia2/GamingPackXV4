namespace Sorry.Core.Data;
[SingletonGame]
public class SorryGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random) : BasicGameContainer<SorryPlayerItem, SorrySaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
{
    public int MovePlayer { get; set; }
    public int PlayerGoingBack { get; set; }
    public AnimateBasicGameBoard Animates { get; set; } = new(aggregator);
    public Func<Task>? DrawClickAsync { get; set; }
    public Func<EnumColorChoice, Task>? HomeClickedAsync { get; set; }
    public Func<int, Task>? SpaceClickedAsync { get; set; }
}