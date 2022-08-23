namespace BasicGameFrameworkLibrary.Core.ViewModels;
public partial class NewRoundViewModel : ScreenViewModel, INewRoundVM, IBlankGameVM
{
    private readonly BasicData _basicData;
    public CommandContainer CommandContainer { get; set; }
    public NewRoundViewModel(CommandContainer command, IEventAggregator aggregator, BasicData basicData) : base(aggregator)
    {
        CommandContainer = command;
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
        return Aggregator.PublishAsync(new NewRoundEventModel()); //this does not care what happens with the new round.
    }
}