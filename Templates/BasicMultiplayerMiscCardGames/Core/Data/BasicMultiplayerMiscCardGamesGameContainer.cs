namespace BasicMultiplayerMiscCardGames.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class BasicMultiplayerMiscCardGamesGameContainer : CardGameContainer<BasicMultiplayerMiscCardGamesCardInformation, BasicMultiplayerMiscCardGamesPlayerItem, BasicMultiplayerMiscCardGamesSaveInfo>
{
    public BasicMultiplayerMiscCardGamesGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<BasicMultiplayerMiscCardGamesCardInformation> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
}