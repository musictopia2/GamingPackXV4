namespace LifeBoardGame.Core.ViewModels;
[InstanceGame]
public class SpinnerViewModel(CommandContainer commandContainer,
    ISpinnerProcesses processes,
    LifeBoardGameGameContainer gameContainer
        )
{
    public CommandContainer CommandContainer { get; set; } = commandContainer;
    public async Task SpinAsync() //no need for the attributes because a different approach is used this time.
    {
        await processes.StartSpinningAsync();
    }
    public LifeBoardGameGameContainer GameContainer { get; } = gameContainer;
}