namespace GoFish.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class GoFishGameContainer : CardGameContainer<RegularSimpleCard, GoFishPlayerItem, GoFishSaveInfo>
{
    public GoFishGameContainer(BasicData basicData,
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
    public Func<Task>? LoadAskScreenAsync { get; set; }
}