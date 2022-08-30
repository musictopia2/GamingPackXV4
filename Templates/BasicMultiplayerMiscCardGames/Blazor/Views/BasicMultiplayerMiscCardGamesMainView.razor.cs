namespace BasicMultiplayerMiscCardGames.Blazor.Views;
public partial class BasicMultiplayerMiscCardGamesMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private BasicMultiplayerMiscCardGamesVMData? _vmData;
    private BasicMultiplayerMiscCardGamesGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa.Resolver!.Resolve<BasicMultiplayerMiscCardGamesVMData>();
        _gameContainer = aa.Resolver.Resolve<BasicMultiplayerMiscCardGamesGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(BasicMultiplayerMiscCardGamesVMData.NormalTurn))
            .AddLabel("Status", nameof(BasicMultiplayerMiscCardGamesVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Cards Left", true, nameof(BasicMultiplayerMiscCardGamesPlayerItem.ObjectCount))

            ; //cards left is common.  can be anything you need.
        base.OnInitialized();
    }
}