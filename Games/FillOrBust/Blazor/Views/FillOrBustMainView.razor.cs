namespace FillOrBust.Blazor.Views;
public partial class FillOrBustMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private FillOrBustVMData? _vmData;
    private FillOrBustGameContainer? _gameContainer;
    private readonly BasicList<LabelGridModel> _temps = new();
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<FillOrBustVMData>();
        _gameContainer = aa1.Resolver.Resolve<FillOrBustGameContainer>();
        _labels.Clear();
        _temps.Clear();
        _labels.AddLabel("Turn", nameof(FillOrBustVMData.NormalTurn))
             .AddLabel("Status", nameof(FillOrBustVMData.Status));
        _temps.AddLabel("Temporary Score", nameof(FillOrBustVMData.TempScore))
            .AddLabel("Score", nameof(FillOrBustVMData.DiceScore));
        _scores.Clear();
        _scores.AddColumn("Current Score", true, nameof(FillOrBustPlayerItem.CurrentScore))
            .AddColumn("Total Score", true, nameof(FillOrBustPlayerItem.TotalScore));
        base.OnInitialized();
    }
    private ICustomCommand RollCommand => DataContext!.RollDiceCommand!;
    private ICustomCommand RemoveCommand => DataContext!.ChooseDiceCommand!;
    private ICustomCommand EndCommand => DataContext!.EndTurnCommand!;
}