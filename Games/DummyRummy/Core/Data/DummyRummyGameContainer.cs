namespace DummyRummy.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class DummyRummyGameContainer : CardGameContainer<RegularRummyCard, DummyRummyPlayerItem, DummyRummySaveInfo>
{
    public DummyRummyGameContainer(BasicData basicData,
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