namespace SorryDicedGame.Core.Data;
[SingletonGame]
public class SorryDicedGameGameContainer : BasicGameContainer<SorryDicedGamePlayerItem, SorryDicedGameSaveInfo>
{
    public SorryDicedGameGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
    {
    }
    public static bool CanStart { get; set; }
}