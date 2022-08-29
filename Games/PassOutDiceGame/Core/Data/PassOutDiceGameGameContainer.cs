namespace PassOutDiceGame.Core.Data;
[SingletonGame]
[AutoReset]
public class PassOutDiceGameGameContainer : BasicGameContainer<PassOutDiceGamePlayerItem, PassOutDiceGameSaveInfo>
{
    public PassOutDiceGameGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
    {
    }
    public Dictionary<int, SpaceInfo>? SpaceList { get; set; }
    public Func<int, Task>? MakeMoveAsync { get; set; }
}