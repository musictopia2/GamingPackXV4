namespace SequenceDice.Core.Data;
[SingletonGame]
public class SequenceDiceGameContainer : BasicGameContainer<SequenceDicePlayerItem, SequenceDiceSaveInfo>
{
    public SequenceDiceGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
    {
    }
}