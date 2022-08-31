namespace FillOrBust.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class FillOrBustGameContainer : CardGameContainer<FillOrBustCardInformation, FillOrBustPlayerItem, FillOrBustSaveInfo>
{
    public FillOrBustGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<FillOrBustCardInformation> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
}