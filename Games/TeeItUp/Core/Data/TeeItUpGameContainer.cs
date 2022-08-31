namespace TeeItUp.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class TeeItUpGameContainer : CardGameContainer<TeeItUpCardInformation, TeeItUpPlayerItem, TeeItUpSaveInfo>
{
    public TeeItUpGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<TeeItUpCardInformation> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
    internal Func<TeeItUpPlayerItem, TeeItUpCardInformation, Task>? BoardClickedAsync { get; set; }
}