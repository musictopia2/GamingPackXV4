namespace DiceBoardGamesMultiplayer.Core.Data;
[SingletonGame]
public class DiceBoardGamesMultiplayerGameContainer : BasicGameContainer<DiceBoardGamesMultiplayerPlayerItem, DiceBoardGamesMultiplayerSaveInfo>
{
    public DiceBoardGamesMultiplayerGameContainer(BasicData basicData,
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