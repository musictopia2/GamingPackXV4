namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.GameContainers;
public interface IBasicGameContainer
{
    IEventAggregator? Aggregator { get; }
    BasicData? BasicData { get; }
    CommandContainer? Command { get; }
    Func<Task>? ContinueTurnAsync { get; set; }
    IAsyncDelayer? Delay { get; }
    Func<Task>? EndTurnAsync { get; set; }
    IGameInfo? GameInfo { get; }
    IGameNetwork? Network { get; }
    IRandomGenerator? Random { get; }
    IGamePackageResolver? Resolver { get; }
    Func<Task>? ShowWinAsync { get; set; }
    TestOptions Test { get; }
    int WhoTurn { get; set; }
}