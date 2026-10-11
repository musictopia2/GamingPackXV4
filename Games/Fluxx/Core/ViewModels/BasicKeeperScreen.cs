namespace Fluxx.Core.ViewModels;
public abstract class BasicKeeperScreen(FluxxGameContainer gameContainer, KeeperContainer keeperContainer)
{
    public CommandContainer CommandContainer { get; set; } = gameContainer.Command;
    protected FluxxGameContainer GameContainer { get; } = gameContainer;
    protected KeeperContainer KeeperContainer { get; } = keeperContainer;
}