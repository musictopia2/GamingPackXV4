namespace DominoBonesMultiplayerGames.Core.Data;
[SingletonGame]
public class DominoBonesMultiplayerGamesGameContainer : BasicGameContainer<DominoBonesMultiplayerGamesPlayerItem, DominoBonesMultiplayerGamesSaveInfo>
{
    public DominoBonesMultiplayerGamesGameContainer(BasicData basicData,
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