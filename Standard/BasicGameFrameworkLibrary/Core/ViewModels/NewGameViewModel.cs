namespace BasicGameFrameworkLibrary.Core.ViewModels;
public partial class NewGameViewModel
{
    private readonly BasicData _basicData;
    private readonly IEventAggregator _aggregator;
    public NewGameViewModel(
        BasicData basicData,
        IEventAggregator aggregator)
    {
        _basicData = basicData;
        _aggregator = aggregator;
        CreateCommands();
    }
    partial void CreateCommands();
    public bool CanStartNewGame() =>
        _basicData.MultiPlayer == false ||
        _basicData.Client == false;

    [Command(EnumCommandCategory.Old)]
    public Task StartNewGameAsync()
    {
        _basicData.GameDataLoading = true;
        return _aggregator.PublishAsync(new NewGameEventModel());
    }
}