namespace Payday.Core.ViewModels;
[InstanceGame]
public class MailListViewModel : ScreenViewModel, IBlankGameVM
{
    public MailListViewModel(CommandContainer commandContainer, IEventAggregator aggregator) : base(aggregator)
    {
        CommandContainer = commandContainer;
    }
    public CommandContainer CommandContainer { get; set; }
}