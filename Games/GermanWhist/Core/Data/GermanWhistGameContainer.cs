namespace GermanWhist.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class GermanWhistGameContainer : TrickGameContainer<GermanWhistCardInformation, GermanWhistPlayerItem, GermanWhistSaveInfo, EnumSuitList>
{
    public GermanWhistGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<GermanWhistCardInformation> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
}