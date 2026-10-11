namespace PickelCardGame.Blazor.Views;
public partial class PickelCardGameMainView
{
    private readonly BasicList<LabelGridModel> _labels = [];
    private readonly BasicList<ScoreColumnModel> _scores = [];
    private PickelCardGameVMData? _vmData;
    private PickelCardGameGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = Resolver!.Resolve<PickelCardGameVMData>();
        _gameContainer = Resolver.Resolve<PickelCardGameGameContainer>();
        _labels.AddLabel("Turn", nameof(PickelCardGameVMData.NormalTurn))
             .AddLabel("Trump", nameof(PickelCardGameVMData.TrumpSuit))
             .AddLabel("Status", nameof(PickelCardGameVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Suit Desired", true, nameof(PickelCardGamePlayerItem.SuitDesired))
            .AddColumn("Bid Amount", false, nameof(PickelCardGamePlayerItem.BidAmount))
            .AddColumn("Tricks Won", false, nameof(PickelCardGamePlayerItem.TricksWon))
            .AddColumn("Current Score", false, nameof(PickelCardGamePlayerItem.CurrentScore))
            .AddColumn("Total Score", false, nameof(PickelCardGamePlayerItem.TotalScore));
        base.OnInitialized();
    }
}