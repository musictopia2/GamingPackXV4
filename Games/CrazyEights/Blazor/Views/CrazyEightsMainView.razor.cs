namespace CrazyEights.Blazor.Views;
public partial class CrazyEightsMainView
{
    private readonly BasicList<LabelGridModel> _labels = [];
    private readonly BasicList<ScoreColumnModel> _scores = [];
    private CrazyEightsVMData? _vmData;
    private CrazyEightsGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<CrazyEightsVMData>();
        _gameContainer = aa1.Resolver.Resolve<CrazyEightsGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(CrazyEightsVMData.NormalTurn))
            .AddLabel("Status", nameof(CrazyEightsVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Cards Left", true, nameof(CrazyEightsPlayerItem.ObjectCount));
        base.OnInitialized();
    }
}