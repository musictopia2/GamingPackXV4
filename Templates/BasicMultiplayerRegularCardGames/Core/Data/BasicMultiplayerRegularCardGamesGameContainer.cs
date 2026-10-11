namespace BasicMultiplayerRegularCardGames.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class BasicMultiplayerRegularCardGamesGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<RegularSimpleCard> deckList,
    IRandomGenerator random) : CardGameContainer<RegularSimpleCard, BasicMultiplayerRegularCardGamesPlayerItem, BasicMultiplayerRegularCardGamesSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
}