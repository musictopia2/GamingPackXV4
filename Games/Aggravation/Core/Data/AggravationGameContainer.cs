namespace Aggravation.Core.Data;
[SingletonGame]
public class AggravationGameContainer : BasicGameContainer<AggravationPlayerItem, AggravationSaveInfo>
{
    public AggravationGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
    {
        Animates = new(aggregator);
    }
    public int MovePlayer { get; set; }
    public int PlayerGoingBack { get; set; }
    public AnimateBasicGameBoard Animates { get; set; }
    public Func<int, bool>? IsValidMove { get; set; }
    public Func<int, Task>? MakeMoveAsync { get; set; }
}