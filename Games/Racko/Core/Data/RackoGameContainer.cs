namespace Racko.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class RackoGameContainer : CardGameContainer<RackoCardInformation, RackoPlayerItem, RackoSaveInfo>
{
    public RackoGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<RackoCardInformation> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
}