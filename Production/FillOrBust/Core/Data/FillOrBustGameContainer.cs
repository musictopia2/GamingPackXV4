namespace FillOrBust.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class FillOrBustGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<FillOrBustCardInformation> deckList,
    IRandomGenerator random) : CardGameContainer<FillOrBustCardInformation, FillOrBustPlayerItem, FillOrBustSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
}