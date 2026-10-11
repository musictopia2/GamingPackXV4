namespace BasicMultiplayerTrickCardGames.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class BasicMultiplayerTrickCardGamesGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<BasicMultiplayerTrickCardGamesCardInformation> deckList,
    IRandomGenerator random) : TrickGameContainer<BasicMultiplayerTrickCardGamesCardInformation, BasicMultiplayerTrickCardGamesPlayerItem, BasicMultiplayerTrickCardGamesSaveInfo, EnumSuitList>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
}