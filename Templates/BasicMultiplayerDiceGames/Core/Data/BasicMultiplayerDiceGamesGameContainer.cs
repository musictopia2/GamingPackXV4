namespace BasicMultiplayerDiceGames.Core.Data;
[SingletonGame]
public class BasicMultiplayerDiceGamesGameContainer : BasicGameContainer<BasicMultiplayerDiceGamesPlayerItem, BasicMultiplayerDiceGamesSaveInfo>
{
    public BasicMultiplayerDiceGamesGameContainer(BasicData basicData,
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