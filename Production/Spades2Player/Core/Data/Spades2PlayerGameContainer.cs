namespace Spades2Player.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class Spades2PlayerGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<Spades2PlayerCardInformation> deckList,
    IRandomGenerator random) : TrickGameContainer<Spades2PlayerCardInformation, Spades2PlayerPlayerItem, Spades2PlayerSaveInfo, EnumSuitList>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
}