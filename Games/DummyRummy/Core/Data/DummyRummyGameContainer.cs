namespace DummyRummy.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class DummyRummyGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<RegularRummyCard> deckList,
    IRandomGenerator random) : CardGameContainer<RegularRummyCard, DummyRummyPlayerItem, DummyRummySaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
    public BasicList<OrganizeModel> TempSets { get; set; } = [];
}