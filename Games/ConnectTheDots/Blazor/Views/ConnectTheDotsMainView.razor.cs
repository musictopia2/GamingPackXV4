namespace ConnectTheDots.Blazor.Views;
public partial class ConnectTheDotsMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private GameBoardGraphicsCP? _graphics;
    private ConnectTheDotsGameContainer? _container;
    protected override void OnInitialized()
    {
        _graphics = aa.Resolver!.Resolve<GameBoardGraphicsCP>();
        _container = aa.Resolver.Resolve<ConnectTheDotsGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(ConnectTheDotsVMData.NormalTurn))
            .AddLabel("Status", nameof(ConnectTheDotsVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Score", true, nameof(ConnectTheDotsPlayerItem.Score));
        base.OnInitialized();
    }
}