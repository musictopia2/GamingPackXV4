namespace OldMaid.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class OldMaidGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<RegularSimpleCard> deckList,
    IRandomGenerator random) : CardGameContainer<RegularSimpleCard, OldMaidPlayerItem, OldMaidSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
    public OldMaidPlayerItem? OtherPlayer;
    internal Func<Task>? ShowOtherCardsAsync { get; set; }
}