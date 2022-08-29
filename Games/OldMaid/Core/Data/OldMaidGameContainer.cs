namespace OldMaid.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class OldMaidGameContainer : CardGameContainer<RegularSimpleCard, OldMaidPlayerItem, OldMaidSaveInfo>
{
    public OldMaidGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<RegularSimpleCard> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
    public OldMaidPlayerItem? OtherPlayer;
    internal Func<Task>? ShowOtherCardsAsync { get; set; }
}