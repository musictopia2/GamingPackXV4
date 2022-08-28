namespace ShipCaptainCrew.Blazor.Views;
public partial class ShipCaptainCrewMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(ShipCaptainCrewVMData.NormalTurn))
                .AddLabel("Roll", nameof(ShipCaptainCrewVMData.RollNumber))
                .AddLabel("Status", nameof(ShipCaptainCrewVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Score", true, nameof(ShipCaptainCrewPlayerItem.Score))
            .AddColumn("Out", true, nameof(ShipCaptainCrewPlayerItem.WentOut), category: EnumScoreSpecialCategory.TrueFalse)
            .AddColumn("Wins", true, nameof(ShipCaptainCrewPlayerItem.Wins));
        base.OnInitialized();
    }
    private ICustomCommand RollCommand => DataContext!.RollDiceCommand!;
}