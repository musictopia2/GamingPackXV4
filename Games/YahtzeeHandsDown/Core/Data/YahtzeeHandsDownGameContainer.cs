namespace YahtzeeHandsDown.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class YahtzeeHandsDownGameContainer : CardGameContainer<YahtzeeHandsDownCardInformation, YahtzeeHandsDownPlayerItem, YahtzeeHandsDownSaveInfo>
{
    public YahtzeeHandsDownGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<YahtzeeHandsDownCardInformation> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
}