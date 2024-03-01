namespace MonopolyDicedGame.Blazor.Views;
public partial class MonopolyDicedGameMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = [];
    private readonly BasicList<ScoreColumnModel> _scores = [];
    private MonopolyDicedGameGameContainer? _container;
    private HouseDice? _house; //hopefully still okay.
    protected override void OnInitialized()
    {
        _container = aa1.Resolver!.Resolve<MonopolyDicedGameGameContainer>();
        _house = aa1.Resolver!.Resolve<HouseDice>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(MonopolyDicedGameVMData.NormalTurn))
            .AddLabel("Status", nameof(MonopolyDicedGameVMData.Status))
            .AddLabel("Roll", nameof(MonopolyDicedGameVMData.RollNumber))
            .AddLabel("Current Score", nameof(MonopolyDicedGameVMData.CurrentScore));
        _scores.Clear();
        _scores.AddColumn("Recent Score", true, nameof(MonopolyDicedGamePlayerItem.CurrentScore))
            .AddColumn("Total Game", true, nameof(MonopolyDicedGamePlayerItem.TotalScore));
        base.OnInitialized();
    }
    private ICustomCommand RollDiceCommand => DataContext!.RollCommand!;
    private ICustomCommand EndTurnCommand => DataContext!.EndTurnCommand!;
    private BasicGameCommand UtilityCommand => DataContext!.UtilityCommand!;
    private BasicGameCommand TrainCommand => DataContext!.TrainCommand!;
    private BasicGameCommand PropertyCommand => DataContext!.PropertyCommand!;
    private async Task PropertyV2Async(int group)
    {
        if (PropertyCommand.CanExecute(group) == false)
        {
            return;
        }
        await PropertyCommand.ExecuteAsync(group);
    }
    private async Task UtilityV2Async(EnumUtilityType utility)
    {
        if (UtilityCommand.CanExecute(utility) == false)
        {
            return;
        }
        await UtilityCommand.ExecuteAsync(utility);
    }
    private async Task TrainV2Async()
    {
        if (TrainCommand.CanExecute(null!) == false)
        {
            return;
        }
        await TrainCommand.ExecuteAsync(null);
    }
}