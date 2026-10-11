namespace SequenceDice.Core.Data;
[SingletonGame]
public class SequenceDiceGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random) : BasicGameContainer<SequenceDicePlayerItem, SequenceDiceSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
{
}