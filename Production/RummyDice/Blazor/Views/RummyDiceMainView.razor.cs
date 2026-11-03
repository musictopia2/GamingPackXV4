namespace RummyDice.Blazor.Views;
public partial class RummyDiceMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = [];
    private readonly BasicList<ScoreColumnModel> _scores = [];
    protected override void OnInitialized()
    {
        string extras = "";
        if (ll1.LayoutMode is not EnumGameLayoutMode.Compact)
        {
            extras = "Score ";
        }
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(RummyDiceVMData.NormalTurn))
            .AddLabel("Status", nameof(RummyDiceVMData.Status))
            .AddLabel("Phase", nameof(RummyDiceVMData.CurrentPhase))
                 .AddLabel("Roll", nameof(RummyDiceVMData.RollNumber))
                 .AddLabel("Score", nameof(RummyDiceVMData.Score));
        _scores.Clear();
        _scores.AddColumn($"{extras}Round", true, nameof(RummyDicePlayerItem.ScoreRound))
            .AddColumn($"{extras}Game", true, nameof(RummyDicePlayerItem.ScoreGame))
            .AddColumn("Phase", true, nameof(RummyDicePlayerItem.Phase));
        base.OnInitialized();
    }
    private ICustomCommand EndTurnCommand => DataContext!.EndTurnCommand!;
    private ICustomCommand PutBackCommand => DataContext!.BoardCommand!;
    private ICustomCommand RollCommand => DataContext!.RollCommand!;
    private ICustomCommand ScoreCommand => DataContext!.CheckCommand!;

    private static double _defaultSize = ll1.LayoutMode == EnumGameLayoutMode.Compact ? 12 : 18;

    private static string HeightPixels => _defaultSize.HeightPixels;

}