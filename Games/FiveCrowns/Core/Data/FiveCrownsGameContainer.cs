namespace FiveCrowns.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class FiveCrownsGameContainer : CardGameContainer<FiveCrownsCardInformation, FiveCrownsPlayerItem, FiveCrownsSaveInfo>
{
    public FiveCrownsGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<FiveCrownsCardInformation> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
}