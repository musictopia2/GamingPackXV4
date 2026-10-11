namespace Pinochle2Player.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class Pinochle2PlayerGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<Pinochle2PlayerCardInformation> deckList,
    IRandomGenerator random) : TrickGameContainer<Pinochle2PlayerCardInformation, Pinochle2PlayerPlayerItem, Pinochle2PlayerSaveInfo, EnumSuitList>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
}