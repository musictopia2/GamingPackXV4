namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.GameContainers;
/// <summary>
/// the purpose of this class is to hold information about a game but not require the game class to stop overflows
/// and to stop lots of repeating.  would be best in the new templates to override this to do other things i want.
/// intended to be used by the game loader to load the new saved data if any.
/// for other cases, intended to be used as helpers to the main game class.
/// allows the possibility for more specialized game classes even like gameboard classes.
/// </summary>
public class BasicGameContainer<P, S>(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random
        )
    : ISaveContainer<P, S>,
    IBasicGameContainer where P : class, IPlayerItem, new()
    where S : BasicSavedGameClass<P>, new()
{
    public S SaveRoot { get; set; } = new S();
    public P? SingleInfo { get; set; } //this is whose turn it is.
    public int WhoTurn
    {
        get => SaveRoot!.PlayOrder.WhoTurn;
        set => SaveRoot!.PlayOrder.WhoTurn = value;
    }
    public int WhoStarts
    {
        get => SaveRoot.PlayOrder.WhoStarts;
        set => SaveRoot.PlayOrder.WhoStarts = value;
    }
    public PlayerCollection<P>? PlayerList => SaveRoot!.PlayerList;
    public IGameNetwork? Network { get; } = basicData.GetNetwork();
    public BasicData BasicData { get; } = basicData;
    public TestOptions Test { get; } = test;
    public IGameInfo GameInfo { get; } = gameInfo;
    public IAsyncDelayer Delay { get; } = delay;
    public IEventAggregator Aggregator { get; } = aggregator;
    public CommandContainer Command { get; } = command;
    public IGamePackageResolver Resolver { get; } = resolver;
    public IRandomGenerator Random { get; } = random;
    public async Task ProcessCustomCommandAsync<T>(Func<T, Task> action, T argument)
    {
        Command.StartExecuting();
        await action.Invoke(argument);

        Command.StopExecuting();
    }
    public async Task ProcessCustomCommandAsync(Func<Task> action)
    {

        Command.StartExecuting();
        await action.Invoke();
        Command.StopExecuting();
    }
    public Func<Task>? EndTurnAsync { get; set; }
    public Func<int, Task>? PlayCardAsync { get; set; }
    public Func<Task>? ContinueTurnAsync { get; set; }
    public Func<Task>? ShowWinAsync { get; set; }
    public Func<Task>? StartNewTurnAsync { get; set; }
    public Func<Task>? SaveStateAsync { get; set; } //games like fluxx requires this.
}