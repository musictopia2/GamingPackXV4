namespace Spades4Player.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class Spades4PlayerGameContainer : TrickGameContainer<Spades4PlayerCardInformation, Spades4PlayerPlayerItem, Spades4PlayerSaveInfo, EnumSuitList>
{
    public Spades4PlayerGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<Spades4PlayerCardInformation> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
}