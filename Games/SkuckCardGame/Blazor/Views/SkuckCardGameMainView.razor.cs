namespace SkuckCardGame.Blazor.Views;
public partial class SkuckCardGameMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private SkuckCardGameVMData? _vmData;
    private SkuckCardGameGameContainer? _gameContainer;
    private BasicList<SkuckCardGamePlayerItem> _players = new();
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<SkuckCardGameVMData>();
        _gameContainer = aa1.Resolver.Resolve<SkuckCardGameGameContainer>();
        _players = _gameContainer.PlayerList!.GetAllPlayersStartingWithSelf();
        _labels.Clear();
        _labels.AddLabel("Round", nameof(SkuckCardGameVMData.RoundNumber))
            .AddLabel("Trump", nameof(SkuckCardGameVMData.TrumpSuit))
            .AddLabel("Turn", nameof(SkuckCardGameVMData.NormalTurn))
            .AddLabel("Status", nameof(SkuckCardGameVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Strength", false, nameof(SkuckCardGamePlayerItem.StrengthHand))
            .AddColumn("Tie", false, nameof(SkuckCardGamePlayerItem.TieBreaker))
            .AddColumn("Bid", false, nameof(SkuckCardGamePlayerItem.BidAmount), visiblePath: nameof(SkuckCardGamePlayerItem.BidVisible))
            .AddColumn("Won", false, nameof(SkuckCardGamePlayerItem.TricksWon))
            .AddColumn("Cards", false, nameof(SkuckCardGamePlayerItem.ObjectCount))
            .AddColumn("P Rounds", false, nameof(SkuckCardGamePlayerItem.PerfectRounds))
            .AddColumn("T Score", false, nameof(SkuckCardGamePlayerItem.TotalScore));
        base.OnInitialized();
    }
}