namespace YachtRace.Blazor.Views;
public partial class YachtRaceMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(YachtRaceVMData.NormalTurn))
            .AddLabel("Status", nameof(YachtRaceVMData.Status))
            .AddLabel("Error Message", nameof(YachtRaceVMData.ErrorMessage));
        _scores.Clear();
        _scores.AddColumn("Time", true, nameof(YachtRacePlayerItem.Time));
        base.OnInitialized();
    }
    private ICustomCommand EndCommand => DataContext!.FiveKindCommand!;
    private ICustomCommand RollCommand => DataContext!.RollDiceCommand!;
}