namespace FourSuitRummy.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class FourSuitRummyGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<RegularRummyCard> deckList,
    IRandomGenerator random) : CardGameContainer<RegularRummyCard, FourSuitRummyPlayerItem, FourSuitRummySaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
    public BasicList<OrganizeModel> TempSets { get; set; } = [];
}