namespace RummyDice.Blazor.Views;
public partial class RummyDiceMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(RummyDiceVMData.NormalTurn))
            .AddLabel("Status", nameof(RummyDiceVMData.Status))
            .AddLabel("Phase", nameof(RummyDiceVMData.CurrentPhase))
                 .AddLabel("Roll", nameof(RummyDiceVMData.RollNumber))
                 .AddLabel("Score", nameof(RummyDiceVMData.Score));
        _scores.Clear();
        _scores.AddColumn("Score Round", true, nameof(RummyDicePlayerItem.ScoreRound))
            .AddColumn("Score Game", true, nameof(RummyDicePlayerItem.ScoreGame))
            .AddColumn("Phase", true, nameof(RummyDicePlayerItem.Phase));
        base.OnInitialized();
    }
    private ICustomCommand EndTurnCommand => DataContext!.EndTurnCommand!;
    private ICustomCommand PutBackCommand => DataContext!.BoardCommand!;
    private ICustomCommand RollCommand => DataContext!.RollCommand!;
    private ICustomCommand ScoreCommand => DataContext!.CheckCommand!;
}