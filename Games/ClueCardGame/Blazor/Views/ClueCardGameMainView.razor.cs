namespace ClueCardGame.Blazor.Views;
public partial class ClueCardGameMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private ClueCardGameVMData? _vmData;
    private ClueCardGameGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<ClueCardGameVMData>();
        _gameContainer = aa1.Resolver.Resolve<ClueCardGameGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(ClueCardGameVMData.NormalTurn))
            .AddLabel("Status", nameof(ClueCardGameVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Cards Left", true, nameof(ClueCardGamePlayerItem.ObjectCount))

            ; //cards left is common.  can be anything you need.
        base.OnInitialized();
    }
}