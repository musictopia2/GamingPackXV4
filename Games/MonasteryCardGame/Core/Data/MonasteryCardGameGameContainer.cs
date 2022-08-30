namespace MonasteryCardGame.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class MonasteryCardGameGameContainer : CardGameContainer<MonasteryCardInfo, MonasteryCardGamePlayerItem, MonasteryCardGameSaveInfo>
{
    public MonasteryCardGameGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<MonasteryCardInfo> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
    internal RummyClass? Rummys { get; set; }
}