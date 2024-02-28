namespace MonasteryCardGame.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class MonasteryCardGameGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<MonasteryCardInfo> deckList,
    IRandomGenerator random) : CardGameContainer<MonasteryCardInfo, MonasteryCardGamePlayerItem, MonasteryCardGameSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
    internal RummyClass? Rummys { get; set; }
    public BasicList<OrganizeModel> TempSets { get; set; } = [];
}