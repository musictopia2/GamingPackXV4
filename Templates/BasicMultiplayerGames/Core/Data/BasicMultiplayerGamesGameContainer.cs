namespace BasicMultiplayerGames.Core.Data;
[SingletonGame]
public class BasicMultiplayerGamesGameContainer : BasicGameContainer<BasicMultiplayerGamesPlayerItem, BasicMultiplayerGamesSaveInfo>
{
    public BasicMultiplayerGamesGameContainer(BasicData basicData,
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