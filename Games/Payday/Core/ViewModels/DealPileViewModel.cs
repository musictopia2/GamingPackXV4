namespace Payday.Core.ViewModels;
[InstanceGame]
public class DealPileViewModel : ScreenViewModel, IBlankGameVM
{
    public DealPileViewModel(CommandContainer commandContainer, IEventAggregator aggregator) : base(aggregator)
    {
        CommandContainer = commandContainer;
    }
    public CommandContainer CommandContainer { get; set; }
}