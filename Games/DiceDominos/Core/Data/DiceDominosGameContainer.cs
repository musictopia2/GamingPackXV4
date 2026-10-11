namespace DiceDominos.Core.Data;
[SingletonGame]
public class DiceDominosGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random,
    DominosBasicShuffler<SimpleDominoInfo> dominosShuffler
        ) : BasicGameContainer<DiceDominosPlayerItem, DiceDominosSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
{
    public DominosBasicShuffler<SimpleDominoInfo> DominosShuffler { get; } = dominosShuffler;
    internal Func<SimpleDominoInfo, Task>? DominoClickedAsync { get; set; } //since i am using a standard control, maybe this will work (?)
}