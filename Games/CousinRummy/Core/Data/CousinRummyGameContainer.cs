namespace CousinRummy.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class CousinRummyGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<RegularRummyCard> deckList,
    IRandomGenerator random) : CardGameContainer<RegularRummyCard, CousinRummyPlayerItem, CousinRummySaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
    internal Action<BasicList<RegularRummyCard>>? ModifyCards { get; set; }
    public BasicList<OrganizeModel> TempSets { get; set; } = [];
}