namespace YaBlewIt.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class YaBlewItGameContainer : CardGameContainer<YaBlewItCardInformation, YaBlewItPlayerItem, YaBlewItSaveInfo>
{
    public YaBlewItGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<YaBlewItCardInformation> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
        SafeColorListClass.ClearContainer(); //because new instance of the container.
    }
}