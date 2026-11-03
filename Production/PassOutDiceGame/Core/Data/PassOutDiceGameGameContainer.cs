namespace PassOutDiceGame.Core.Data;
[SingletonGame]
[AutoReset]
public class PassOutDiceGameGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random) : BasicGameContainer<PassOutDiceGamePlayerItem, PassOutDiceGameSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
{
    public Dictionary<int, SpaceInfo>? SpaceList { get; set; }
    public Func<int, Task>? MakeMoveAsync { get; set; }
}