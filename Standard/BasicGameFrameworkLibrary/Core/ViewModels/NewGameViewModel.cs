namespace BasicGameFrameworkLibrary.Core.ViewModels;

public sealed partial class NewGameViewModel : ScreenViewModel, INewGameVM, IBlankGameVM
{
    private readonly BasicData _basicData;
    public CommandContainer CommandContainer { get; set; }
    public NewGameViewModel(CommandContainer command, BasicData basicData, IEventAggregator aggregator) : base(aggregator)
    {
        CommandContainer = command;
        _basicData = basicData;
        CreateCommands();
    }
    protected override Task TryCloseAsync()
    {
        return base.TryCloseAsync();
    }
    partial void CreateCommands();
    public bool CanStartNewGame() => _basicData.MultiPlayer == false || _basicData.Client == false;
    [Command(EnumCommandCategory.Old)] //try old.
    public Task StartNewGameAsync()
    {
        _basicData.GameDataLoading = true; //hopefully this simple (?)
        return Aggregator.PublishAsync(new NewGameEventModel()); //this does not care what happens with the new game.
    }
}