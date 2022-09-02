namespace RageCardGame.Core.ViewModels;
[InstanceGame]
public class RageColorViewModel : ScreenViewModel, IBlankGameVM
{
    public readonly RageCardGameVMData Model;
    public RageColorViewModel(CommandContainer commandContainer, RageCardGameVMData model, RageCardGameGameContainer gameContainer, IEventAggregator aggregator) : base(aggregator)
    {
        CommandContainer = commandContainer;
        Model = model;
        GameContainer = gameContainer;
    }
    public CommandContainer CommandContainer { get; set; }
    public RageCardGameGameContainer GameContainer { get; }
}