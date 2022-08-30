namespace FourSuitRummy.Core.ViewModels;
[InstanceGame]
public class PlayerSetsViewModel : ScreenViewModel, IBlankGameVM
{
    public PlayerSetsViewModel(CommandContainer commandContainer, IEventAggregator aggregator) : base(aggregator)
    {
        CommandContainer = commandContainer;
    }
    public CommandContainer CommandContainer { get; set; }
}