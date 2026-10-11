namespace ItalianDominos.Core.Data;
[SingletonGame]
public class ItalianDominosGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random) : BasicGameContainer<ItalianDominosPlayerItem, ItalianDominosSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
{
}