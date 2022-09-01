namespace Pinochle2Player.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class Pinochle2PlayerGameContainer : TrickGameContainer<Pinochle2PlayerCardInformation, Pinochle2PlayerPlayerItem, Pinochle2PlayerSaveInfo, EnumSuitList>
{
    public Pinochle2PlayerGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<Pinochle2PlayerCardInformation> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
}