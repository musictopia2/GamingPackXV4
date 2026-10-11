namespace BasicMultiplayerDiceGames.Blazor.Views;
public partial class BasicMultiplayerDiceGamesMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = [];
    private readonly BasicList<ScoreColumnModel> _scores = [];
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(BasicMultiplayerDiceGamesVMData.NormalTurn))
                .AddLabel("Roll", nameof(BasicMultiplayerDiceGamesVMData.RollNumber))
                .AddLabel("Status", nameof(BasicMultiplayerDiceGamesVMData.Status));
        _scores.Clear();
        //use addcolumn for the columns to add.
        base.OnInitialized();
    }
    private ICustomCommand EndCommand => DataContext!.EndTurnCommand!;
    private ICustomCommand RollCommand => DataContext!.RollDiceCommand!;
}