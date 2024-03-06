namespace DealCardGame.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class DealCardGameGameContainer : CardGameContainer<DealCardGameCardInformation, DealCardGamePlayerItem, DealCardGameSaveInfo>
{
    public DealCardGameGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<DealCardGameCardInformation> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
}