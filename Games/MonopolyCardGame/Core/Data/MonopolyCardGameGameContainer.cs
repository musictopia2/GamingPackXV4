namespace MonopolyCardGame.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class MonopolyCardGameGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<MonopolyCardGameCardInformation> deckList,
    IRandomGenerator random) : CardGameContainer<MonopolyCardGameCardInformation, MonopolyCardGamePlayerItem, MonopolyCardGameSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
    internal Action<TradePile, DeckRegularDict<MonopolyCardGameCardInformation>, TradePile>? ProcessTrade { get; set; }
    public Action<MonopolyCardGamePlayerItem>? StartCustomTrade { get; set; }
    public BasicList<OrganizeModel> TempSets { get; set; } = [];
}