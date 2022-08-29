namespace Payday.Core.ViewModels;
[InstanceGame]
public class MailPileViewModel : ScreenViewModel, IBlankGameVM
{
    public MailPileViewModel(CommandContainer commandContainer, IEventAggregator aggregator) : base(aggregator)
    {
        CommandContainer = commandContainer;
    }
    public CommandContainer CommandContainer { get; set; }
}