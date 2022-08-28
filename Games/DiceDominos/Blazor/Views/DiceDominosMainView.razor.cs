namespace DiceDominos.Blazor.Views;
public partial class DiceDominosMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(DiceDominosVMData.NormalTurn))
                .AddLabel("Status", nameof(DiceDominosVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Dominos Won", true, nameof(DiceDominosPlayerItem.DominosWon));
        base.OnInitialized();
    }
    private ICustomCommand EndCommand => DataContext!.EndTurnCommand!;
    private ICustomCommand RollCommand => DataContext!.RollDiceCommand!;
}