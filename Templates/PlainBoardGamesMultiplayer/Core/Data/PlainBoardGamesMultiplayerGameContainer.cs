namespace PlainBoardGamesMultiplayer.Core.Data;
[SingletonGame]
public class PlainBoardGamesMultiplayerGameContainer : BasicGameContainer<PlainBoardGamesMultiplayerPlayerItem, PlainBoardGamesMultiplayerSaveInfo>
{
    public PlainBoardGamesMultiplayerGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
    {
    }
}