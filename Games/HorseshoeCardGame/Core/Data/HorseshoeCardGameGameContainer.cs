namespace HorseshoeCardGame.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class HorseshoeCardGameGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<HorseshoeCardGameCardInformation> deckList,
    IRandomGenerator random) : TrickGameContainer<HorseshoeCardGameCardInformation, HorseshoeCardGamePlayerItem, HorseshoeCardGameSaveInfo, EnumSuitList>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
    internal Func<DeckRegularDict<HorseshoeCardGameCardInformation>>? GetCurrentHandList { get; set; }
}