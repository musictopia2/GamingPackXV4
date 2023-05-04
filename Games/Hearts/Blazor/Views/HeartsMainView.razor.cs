namespace Hearts.Blazor.Views;
public partial class HeartsMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private HeartsVMData? _vmData;
    private HeartsGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<HeartsVMData>();
        _gameContainer = aa1.Resolver.Resolve<HeartsGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(HeartsVMData.NormalTurn))
            .AddLabel("Trump", nameof(HeartsVMData.TrumpSuit))
            .AddLabel("Status", nameof(HeartsVMData.Status));

        _scores.Clear();
        _scores.AddColumn("Cards Left", true, nameof(HeartsPlayerItem.ObjectCount))
            .AddColumn("Tricks Won", true, nameof(HeartsPlayerItem.TricksWon))
            ; //cards left is common.  can be anything you need.
        base.OnInitialized();
    }
}