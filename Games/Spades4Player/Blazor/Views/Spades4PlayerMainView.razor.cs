namespace Spades4Player.Blazor.Views;
public partial class Spades4PlayerMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private Spades4PlayerVMData? _vmData;
    private Spades4PlayerGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<Spades4PlayerVMData>();
        _gameContainer = aa1.Resolver.Resolve<Spades4PlayerGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(Spades4PlayerVMData.NormalTurn))
            .AddLabel("Trump", nameof(Spades4PlayerVMData.TrumpSuit))
            .AddLabel("Status", nameof(Spades4PlayerVMData.Status));

        _scores.Clear();
        _scores.AddColumn("Cards Left", true, nameof(Spades4PlayerPlayerItem.ObjectCount))
            .AddColumn("Tricks Won", true, nameof(Spades4PlayerPlayerItem.TricksWon))
            ; //cards left is common.  can be anything you need.
        base.OnInitialized();
    }
}