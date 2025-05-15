namespace Savannah.Blazor.Views;
public partial class SavannahMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    [CascadingParameter]
    public int TargetHeight { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private SavannahVMData? _vmData;
    private SavannahGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<SavannahVMData>();
        _gameContainer = aa1.Resolver.Resolve<SavannahGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(SavannahVMData.NormalTurn))
            .AddLabel("Status", nameof(SavannahVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Cards Left", true, nameof(SavannahPlayerItem.ObjectCount))
            .AddColumn("Discard Left", true, nameof(SavannahPlayerItem.DiscardLeft))
            .AddColumn("Reserve Left", true, nameof(SavannahPlayerItem.ReserveLeft))
            .AddColumn("Next Reserve", true, nameof(SavannahPlayerItem.NextReserve));
        base.OnInitialized();
    }
    private BasicList<SavannahPlayerItem> Opponents
    {
        get
        {
            var output = _gameContainer!.PlayerList!.GetAllPlayersStartingWithSelf();
            output.RemoveFirstItem();
            return output;
        }
    }
    private ICustomCommand PlayerCommand => DataContext!.ClickPlayerDiscardCommand!;
    private string HeightString => $"{TargetHeight}vh";
    private PointF Location => new(10, -300);
}