namespace CaliforniaJack.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class CaliforniaJackGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<CaliforniaJackCardInformation> deckList,
    IRandomGenerator random) : TrickGameContainer<CaliforniaJackCardInformation, CaliforniaJackPlayerItem, CaliforniaJackSaveInfo, EnumSuitList>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
}