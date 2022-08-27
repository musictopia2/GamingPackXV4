namespace ItalianDominos.Core.Data;
[SingletonGame]
public class ItalianDominosGameContainer : BasicGameContainer<ItalianDominosPlayerItem, ItalianDominosSaveInfo>
{
    public ItalianDominosGameContainer(BasicData basicData,
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