namespace Rummy500.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class Rummy500GameContainer : CardGameContainer<RegularRummyCard, Rummy500PlayerItem, Rummy500SaveInfo>
{
    public Rummy500GameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<RegularRummyCard> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
}