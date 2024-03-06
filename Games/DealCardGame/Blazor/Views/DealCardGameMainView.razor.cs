namespace DealCardGame.Blazor.Views;
public partial class DealCardGameMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private DealCardGameVMData? _vmData;
    private DealCardGameGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<DealCardGameVMData>();
        _gameContainer = aa1.Resolver.Resolve<DealCardGameGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(DealCardGameVMData.NormalTurn))
            .AddLabel("Status", nameof(DealCardGameVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Cards Left", true, nameof(DealCardGamePlayerItem.ObjectCount))

            ; //cards left is common.  can be anything you need.
        base.OnInitialized();
    }
}