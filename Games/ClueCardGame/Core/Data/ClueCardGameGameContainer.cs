namespace ClueCardGame.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class ClueCardGameGameContainer : CardGameContainer<ClueCardGameCardInformation, ClueCardGamePlayerItem, ClueCardGameSaveInfo>
{
    public ClueCardGameGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<ClueCardGameCardInformation> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
}