namespace MonopolyCardGame.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class MonopolyCardGameGameContainer : CardGameContainer<MonopolyCardGameCardInformation, MonopolyCardGamePlayerItem, MonopolyCardGameSaveInfo>
{
    public MonopolyCardGameGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<MonopolyCardGameCardInformation> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
    internal Action<TradePile, DeckRegularDict<MonopolyCardGameCardInformation>, TradePile>? ProcessTrade { get; set; }
}