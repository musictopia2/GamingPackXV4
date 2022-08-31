namespace A8RoundRummy.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class A8RoundRummyGameContainer : CardGameContainer<A8RoundRummyCardInformation, A8RoundRummyPlayerItem, A8RoundRummySaveInfo>
{
    public A8RoundRummyGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<A8RoundRummyCardInformation> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
}