namespace BuncoDiceGame.Core.ViewModels;
[InstanceGame]
public partial class BuncoNewRoundViewModel : ScreenViewModel, IBlankGameVM
{
    public BuncoNewRoundViewModel(CommandContainer commandContainer, IEventAggregator aggregator) : base(aggregator)
    {
        CommandContainer = commandContainer;
        commandContainer.ManuelFinish = false;
        commandContainer.IsExecuting = false;
        CreateCommands();
    }
    partial void CreateCommands();
    public CommandContainer CommandContainer { get; set; }
    [Command(EnumCommandCategory.Old)]
    public async Task NewRoundAsync()
    {
        await Aggregator.PublishAllAsync(new ChoseNewRoundEventModel()); //because for sure needs to go to 2 different processes.
    }
}