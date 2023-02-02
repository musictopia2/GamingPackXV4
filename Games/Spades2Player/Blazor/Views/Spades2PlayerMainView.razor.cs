namespace Spades2Player.Blazor.Views;
public partial class Spades2PlayerMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private Spades2PlayerVMData? _vmData;
    private Spades2PlayerGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<Spades2PlayerVMData>();
        _gameContainer = aa1.Resolver.Resolve<Spades2PlayerGameContainer>();
        _labels.AddLabel("Turn", nameof(Spades2PlayerVMData.NormalTurn))
            .AddLabel("Status", nameof(Spades2PlayerVMData.Status))
            .AddLabel("Round", nameof(Spades2PlayerVMData.RoundNumber));
        _scores.Clear();
        _scores.AddColumn("Cards", false, nameof(Spades2PlayerPlayerItem.ObjectCount))
            .AddColumn("Bidded", false, nameof(Spades2PlayerPlayerItem.HowManyBids))
            .AddColumn("Won", false, nameof(Spades2PlayerPlayerItem.TricksWon))
            .AddColumn("Bags", false, nameof(Spades2PlayerPlayerItem.Bags))
            .AddColumn("C Score", false, nameof(Spades2PlayerPlayerItem.CurrentScore))
            .AddColumn("T Score", false, nameof(Spades2PlayerPlayerItem.TotalScore));
        base.OnInitialized();
    }
}