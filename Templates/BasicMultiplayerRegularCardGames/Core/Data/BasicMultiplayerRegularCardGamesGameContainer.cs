namespace BasicMultiplayerRegularCardGames.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class BasicMultiplayerRegularCardGamesGameContainer : CardGameContainer<RegularSimpleCard, BasicMultiplayerRegularCardGamesPlayerItem, BasicMultiplayerRegularCardGamesSaveInfo>
{
    public BasicMultiplayerRegularCardGamesGameContainer(BasicData basicData,
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
}