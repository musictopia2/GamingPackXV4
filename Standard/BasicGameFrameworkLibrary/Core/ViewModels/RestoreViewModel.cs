namespace BasicGameFrameworkLibrary.Core.ViewModels;
public partial class RestoreViewModel : IRestoreVM
{
    private readonly IEventAggregator _aggregator;
    public CommandContainer CommandContainer { get; set; }
    public RestoreViewModel(CommandContainer command, IEventAggregator aggregator)
    {
        CommandContainer = command;
        _aggregator = aggregator;
        CreateCommands();
    }
    partial void CreateCommands();
    [Command(EnumCommandCategory.Old)]
    public Task RestoreAsync()
    {
        return _aggregator.PublishAsync(new RestoreEventModel());
    }
}