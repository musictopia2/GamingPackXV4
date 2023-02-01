namespace RollEm.Blazor.Views;
public partial class RollEmMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private GameBoardGraphicsCP? _boardData;
    protected override void OnInitialized()
    {
        _boardData = aa1.Resolver!.Resolve<GameBoardGraphicsCP>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(RollEmVMData.NormalTurn))
                .AddLabel("Round", nameof(RollEmVMData.Round))
                .AddLabel("Status", nameof(RollEmVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Score Round", true, nameof(RollEmPlayerItem.ScoreRound))
                .AddColumn("Score Game", true, nameof(RollEmPlayerItem.ScoreGame));
        base.OnInitialized();
    }
    private ICustomCommand EndCommand => DataContext!.EndTurnCommand!;
    private ICustomCommand RollCommand => DataContext!.RollDiceCommand!;
}