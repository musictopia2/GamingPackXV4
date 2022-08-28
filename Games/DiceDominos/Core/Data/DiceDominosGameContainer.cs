namespace DiceDominos.Core.Data;
[SingletonGame]
public class DiceDominosGameContainer : BasicGameContainer<DiceDominosPlayerItem, DiceDominosSaveInfo>
{
    public DiceDominosGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IRandomGenerator random,
        DominosBasicShuffler<SimpleDominoInfo> dominosShuffler
        ) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
    {
        DominosShuffler = dominosShuffler;
    }
    public DominosBasicShuffler<SimpleDominoInfo> DominosShuffler { get; } //hopefully this simple.
    internal Func<SimpleDominoInfo, Task>? DominoClickedAsync { get; set; } //since i am using a standard control, maybe this will work (?)
}