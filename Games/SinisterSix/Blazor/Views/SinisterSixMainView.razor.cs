namespace SinisterSix.Blazor.Views;
public partial class SinisterSixMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(SinisterSixVMData.NormalTurn))
                .AddLabel("Roll", nameof(SinisterSixVMData.RollNumber))
                .AddLabel("Status", nameof(SinisterSixVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Score", true, nameof(SinisterSixPlayerItem.Score));
        base.OnInitialized();
    }
    private ICustomCommand EndCommand => DataContext!.EndTurnCommand!;
    private ICustomCommand RollCommand => DataContext!.RollDiceCommand!;
    private ICustomCommand RemoveCommand => DataContext!.RemoveDiceCommand!;
}