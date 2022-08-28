namespace LifeBoardGame.Core.ViewModels;
[InstanceGame]
public class SpinnerViewModel : ScreenViewModel, IBlankGameVM
{
    private readonly ISpinnerProcesses _processes;
    public SpinnerViewModel(CommandContainer commandContainer,
        ISpinnerProcesses processes,
        LifeBoardGameGameContainer gameContainer,
        IEventAggregator aggregator
        ) : base(aggregator)
    {
        CommandContainer = commandContainer;
        _processes = processes;
        GameContainer = gameContainer;
    }
    public CommandContainer CommandContainer { get; set; }
    public async Task SpinAsync() //no need for the attributes because a different approach is used this time.
    {
        await _processes.StartSpinningAsync();
    }
    public LifeBoardGameGameContainer GameContainer { get; }
}