namespace RoundsCardGame.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class RoundsCardGameGameContainer : TrickGameContainer<RoundsCardGameCardInformation, RoundsCardGamePlayerItem, RoundsCardGameSaveInfo, EnumSuitList>
{
    public RoundsCardGameGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<RoundsCardGameCardInformation> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
}