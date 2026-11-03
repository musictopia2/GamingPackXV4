namespace GermanWhist.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class GermanWhistGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<GermanWhistCardInformation> deckList,
    IRandomGenerator random) : TrickGameContainer<GermanWhistCardInformation, GermanWhistPlayerItem, GermanWhistSaveInfo, EnumSuitList>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
}