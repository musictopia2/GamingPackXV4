namespace CoveredUp.Blazor.Views;
public partial class CoveredUpMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private CoveredUpVMData? _vmData;
    private CoveredUpGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa.Resolver!.Resolve<CoveredUpVMData>();
        _gameContainer = aa.Resolver.Resolve<CoveredUpGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(CoveredUpVMData.NormalTurn))
            .AddLabel("Round", nameof(CoveredUpVMData.Round))
            .AddLabel("Status", nameof(CoveredUpVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Points So Far", true, nameof(CoveredUpPlayerItem.PointsSoFar))
            .AddColumn("Previous Score", true, nameof(CoveredUpPlayerItem.PreviousScore))
            .AddColumn("Total Score", true, nameof(CoveredUpPlayerItem.TotalScore));
        base.OnInitialized();
    }
}