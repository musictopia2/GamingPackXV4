namespace LottoDominos.Blazor.Views;
public partial class LottoDominosMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(LottoDominosVMData.NormalTurn))
            .AddLabel("Status", nameof(LottoDominosVMData.Status));
        _scores.Clear();
        _scores.AddColumn("# Chosen", true, nameof(LottoDominosPlayerItem.NumberChosen))
            .AddColumn("# Won", true, nameof(LottoDominosPlayerItem.NumberWon));
        base.OnInitialized();
    }
}