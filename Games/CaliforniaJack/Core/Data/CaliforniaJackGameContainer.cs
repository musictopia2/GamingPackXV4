namespace CaliforniaJack.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class CaliforniaJackGameContainer : TrickGameContainer<CaliforniaJackCardInformation, CaliforniaJackPlayerItem, CaliforniaJackSaveInfo, EnumSuitList>
{
    public CaliforniaJackGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<CaliforniaJackCardInformation> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
}