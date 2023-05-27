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
            .AddLabel("Status", nameof(Spades4PlayerVMData.Status))
            .AddLabel("Team", nameof(Spades4PlayerVMData.TeamMate));
        //since trump is obviously always spades.
        _scores.AddColumn("Cards", false, nameof(Spades4PlayerPlayerItem.ObjectCount))
             .AddColumn("Bidded", false, nameof(Spades4PlayerPlayerItem.HowManyBids))
             .AddColumn("Won", false, nameof(Spades4PlayerPlayerItem.TricksWon))
             .AddColumn("Bags", false, nameof(Spades4PlayerPlayerItem.Bags))
             .AddColumn("C Score", false, nameof(Spades4PlayerPlayerItem.CurrentScore))
             .AddColumn("T Score", false, nameof(Spades4PlayerPlayerItem.TotalScore));
        base.OnInitialized();
    }
}