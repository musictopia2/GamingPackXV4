namespace DeadDie96.Blazor.Views;
public partial class DeadDie96MainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(DeadDie96VMData.NormalTurn))
                //.AddLabel("Roll", nameof(DeadDie96VMData.RollNumber))
                .AddLabel("Status", nameof(DeadDie96VMData.Status));
        _scores.Clear();
        _scores.AddColumn("Current Score", true, nameof(DeadDie96PlayerItem.CurrentScore))
            .AddColumn("Total Score", true, nameof(DeadDie96PlayerItem.TotalScore));
        base.OnInitialized();
    }
    private ICustomCommand RollCommand => DataContext!.RollDiceCommand!;
}