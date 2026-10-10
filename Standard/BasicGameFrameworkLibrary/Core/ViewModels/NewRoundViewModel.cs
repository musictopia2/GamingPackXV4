namespace BasicGameFrameworkLibrary.Core.ViewModels;
public partial class NewRoundViewModel
{
    private readonly IEventAggregator _aggregator;
    private readonly BasicData _basicData;
    public CommandContainer CommandContainer { get; set; }
    public NewRoundViewModel(CommandContainer command, IEventAggregator aggregator, BasicData basicData)
    {
        CommandContainer = command;
        _aggregator = aggregator;
        _basicData = basicData;
        CreateCommands();
    }
    partial void CreateCommands();
    public bool CanStartNewRound
    {
        get
        {
            if (_basicData.MultiPlayer == false)
            {
                return true;
            }
            return _basicData.Client == false;
        }
    }
    [Command(EnumCommandCategory.Old)]
    public Task StartNewRoundAsync()
    {
        return _aggregator.PublishAsync(new NewRoundEventModel());
    }
}