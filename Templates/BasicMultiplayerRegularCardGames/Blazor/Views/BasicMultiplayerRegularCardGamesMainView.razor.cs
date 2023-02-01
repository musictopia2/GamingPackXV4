namespace BasicMultiplayerRegularCardGames.Blazor.Views;
public partial class BasicMultiplayerRegularCardGamesMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private BasicMultiplayerRegularCardGamesVMData? _vmData;
    private BasicMultiplayerRegularCardGamesGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<BasicMultiplayerRegularCardGamesVMData>();
        _gameContainer = aa1.Resolver.Resolve<BasicMultiplayerRegularCardGamesGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(BasicMultiplayerRegularCardGamesVMData.NormalTurn))
            .AddLabel("Status", nameof(BasicMultiplayerRegularCardGamesVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Cards Left", true, nameof(BasicMultiplayerRegularCardGamesPlayerItem.ObjectCount))

            ; //cards left is common.  can be anything you need.
        base.OnInitialized();
    }
}