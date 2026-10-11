namespace BasicMultiplayerTrickCardGames.Blazor.Views;
public partial class BasicMultiplayerTrickCardGamesMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = [];
    private readonly BasicList<ScoreColumnModel> _scores = [];
    private BasicMultiplayerTrickCardGamesVMData? _vmData;
    private BasicMultiplayerTrickCardGamesGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = Resolver!.Resolve<BasicMultiplayerTrickCardGamesVMData>();
        _gameContainer = Resolver.Resolve<BasicMultiplayerTrickCardGamesGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(BasicMultiplayerTrickCardGamesVMData.NormalTurn))
            .AddLabel("Trump", nameof(BasicMultiplayerTrickCardGamesVMData.TrumpSuit))
            .AddLabel("Status", nameof(BasicMultiplayerTrickCardGamesVMData.Status));

        _scores.Clear();
        _scores.AddColumn("Cards Left", true, nameof(BasicMultiplayerTrickCardGamesPlayerItem.ObjectCount))
            .AddColumn("Tricks Won", true, nameof(BasicMultiplayerTrickCardGamesPlayerItem.TricksWon))
            ; //cards left is common.  can be anything you need.
        base.OnInitialized();
    }
}