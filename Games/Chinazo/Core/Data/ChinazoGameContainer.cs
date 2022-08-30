namespace Chinazo.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class ChinazoGameContainer : CardGameContainer<ChinazoCard, ChinazoPlayerItem, ChinazoSaveInfo>
{
    public ChinazoGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<ChinazoCard> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
    internal Action<BasicList<ChinazoCard>>? ModifyCards { get; set; }
}