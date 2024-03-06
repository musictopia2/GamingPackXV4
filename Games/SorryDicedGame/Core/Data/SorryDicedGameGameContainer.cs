namespace SorryDicedGame.Core.Data;
[SingletonGame]
public class SorryDicedGameGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random) : BasicGameContainer<SorryDicedGamePlayerItem, SorryDicedGameSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
{
    public static bool CanStart { get; set; }
    public static bool IsGameOver { get; set; }
}