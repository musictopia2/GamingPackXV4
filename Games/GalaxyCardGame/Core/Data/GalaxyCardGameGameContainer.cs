namespace GalaxyCardGame.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class GalaxyCardGameGameContainer : TrickGameContainer<GalaxyCardGameCardInformation, GalaxyCardGamePlayerItem, GalaxyCardGameSaveInfo, EnumSuitList>
{
    public GalaxyCardGameGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<GalaxyCardGameCardInformation> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
}