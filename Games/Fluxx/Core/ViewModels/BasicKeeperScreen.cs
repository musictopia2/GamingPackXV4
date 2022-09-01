namespace Fluxx.Core.ViewModels;
public abstract class BasicKeeperScreen : ScreenViewModel, IBlankGameVM
{
    public BasicKeeperScreen(FluxxGameContainer gameContainer, KeeperContainer keeperContainer, IEventAggregator aggregator) : base(aggregator)
    {
        GameContainer = gameContainer;
        KeeperContainer = keeperContainer;
        CommandContainer = gameContainer.Command;
    }
    public CommandContainer CommandContainer { get; set; }
    protected FluxxGameContainer GameContainer { get; }
    protected KeeperContainer KeeperContainer { get; }
}