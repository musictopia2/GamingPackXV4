namespace BasicMultiplayerTrickCardGames.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class BasicMultiplayerTrickCardGamesGameContainer : TrickGameContainer<BasicMultiplayerTrickCardGamesCardInformation, BasicMultiplayerTrickCardGamesPlayerItem, BasicMultiplayerTrickCardGamesSaveInfo, EnumSuitList>
{
    public BasicMultiplayerTrickCardGamesGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<BasicMultiplayerTrickCardGamesCardInformation> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
}