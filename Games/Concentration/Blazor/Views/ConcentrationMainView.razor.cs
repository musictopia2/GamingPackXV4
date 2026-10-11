namespace Concentration.Blazor.Views;
public partial class ConcentrationMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = [];
    private readonly BasicList<ScoreColumnModel> _scores = [];
    private ConcentrationVMData? _vmData;
    private ConcentrationGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = Resolver!.Resolve<ConcentrationVMData>();
        _gameContainer = Resolver.Resolve<ConcentrationGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(ConcentrationVMData.NormalTurn))
            .AddLabel("Status", nameof(ConcentrationVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Pairs", true, nameof(ConcentrationPlayerItem.Pairs));
        base.OnInitialized();
    }
}