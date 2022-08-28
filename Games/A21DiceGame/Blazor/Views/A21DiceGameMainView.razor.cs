namespace A21DiceGame.Blazor.Views;
public partial class A21DiceGameMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(A21DiceGameVMData.NormalTurn))
                .AddLabel("Status", nameof(A21DiceGameVMData.Status));
        _scores.Clear();
        _scores.AddColumn("# Of Rolls", true, nameof(A21DiceGamePlayerItem.NumberOfRolls))
               .AddColumn("Score", true, nameof(A21DiceGamePlayerItem.Score))
               .AddColumn("Was Tie", true, nameof(A21DiceGamePlayerItem.IsFaceOff), category: EnumScoreSpecialCategory.TrueFalse);
        //use addcolumn for the columns to add.
        base.OnInitialized();
    }
    private ICustomCommand EndCommand => DataContext!.EndTurnCommand!;
    private ICustomCommand RollCommand => DataContext!.RollDiceCommand!;
}