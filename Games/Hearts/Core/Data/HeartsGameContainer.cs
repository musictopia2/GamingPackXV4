namespace Hearts.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class HeartsGameContainer : TrickGameContainer<HeartsCardInformation, HeartsPlayerItem, HeartsSaveInfo, EnumSuitList>
{
    public HeartsGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<HeartsCardInformation> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
}