namespace FiveCrowns.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class FiveCrownsGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<FiveCrownsCardInformation> deckList,
    IRandomGenerator random) : CardGameContainer<FiveCrownsCardInformation, FiveCrownsPlayerItem, FiveCrownsSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
    public BasicList<OrganizeModel> TempSets { get; set; } = [];
}