namespace DealCardGame.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class DealCardGameGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<DealCardGameCardInformation> deckList,
    IRandomGenerator random) : CardGameContainer<DealCardGameCardInformation, DealCardGamePlayerItem, DealCardGameSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
    public PrivateModel PersonalInformation = new();
    //public Func<BasicList<int>, Task>? Payments { get; set; }
}