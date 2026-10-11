namespace GoFish.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class GoFishGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<RegularSimpleCard> deckList,
    IRandomGenerator random) : CardGameContainer<RegularSimpleCard, GoFishPlayerItem, GoFishSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
    public Action? LoadAskScreenAsync { get; set; }
}