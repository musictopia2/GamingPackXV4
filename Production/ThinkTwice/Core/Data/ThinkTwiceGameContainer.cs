namespace ThinkTwice.Core.Data;
[SingletonGame]
public class ThinkTwiceGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random) : BasicGameContainer<ThinkTwicePlayerItem, ThinkTwiceSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
{
    public Func<Task>? ScoreClickAsync { get; set; } //may have to be public so blazor can use it (?)
    public Func<Task>? CategoryClicked { get; set; }
}