namespace YahtzeeHandsDown.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class YahtzeeHandsDownGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<YahtzeeHandsDownCardInformation> deckList,
    IRandomGenerator random) : CardGameContainer<YahtzeeHandsDownCardInformation, YahtzeeHandsDownPlayerItem, YahtzeeHandsDownSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
}