namespace Aggravation.Core.Data;
[SingletonGame]
public class AggravationGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random) : BasicGameContainer<AggravationPlayerItem, AggravationSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
{
    public int MovePlayer { get; set; }
    public int PlayerGoingBack { get; set; }
    public AnimateBasicGameBoard Animates { get; set; } = new(aggregator);
    public Func<int, bool>? IsValidMove { get; set; }
    public Func<int, Task>? MakeMoveAsync { get; set; }
}