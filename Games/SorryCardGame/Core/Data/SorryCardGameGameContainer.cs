namespace SorryCardGame.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class SorryCardGameGameContainer : CardGameContainer<SorryCardGameCardInformation, SorryCardGamePlayerItem, SorryCardGameSaveInfo>
{
    public SorryCardGameGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<SorryCardGameCardInformation> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
}