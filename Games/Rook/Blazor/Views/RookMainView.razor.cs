namespace Rook.Blazor.Views;
public partial class RookMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private RookVMData? _vmData;
    private RookGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<RookVMData>();
        _gameContainer = aa1.Resolver.Resolve<RookGameContainer>();
        _labels.AddLabel("Turn", nameof(RookVMData.NormalTurn))
            .AddLabel("Trump", nameof(RookVMData.TrumpSuit))
            .AddLabel("Status", nameof(RookVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Bid Amount", true, nameof(RookPlayerItem.BidAmount))
            .AddColumn("Tricks Won", false, nameof(RookPlayerItem.TricksWon))
            .AddColumn("Current Score", false, nameof(RookPlayerItem.CurrentScore))
            .AddColumn("Total Score", false, nameof(RookPlayerItem.TotalScore));
        base.OnInitialized();
    }
}