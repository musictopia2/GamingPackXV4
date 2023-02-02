namespace HorseshoeCardGame.Blazor.Views;
public partial class HorseshoeCardGameMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private HorseshoeCardGameVMData? _vmData;
    private HorseshoeCardGameGameContainer? _gameContainer;
    private BasicList<HorseshoeCardGamePlayerItem> _players = new();
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<HorseshoeCardGameVMData>();
        _gameContainer = aa1.Resolver.Resolve<HorseshoeCardGameGameContainer>();
        _players = _gameContainer.PlayerList!.GetAllPlayersStartingWithSelf();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(HorseshoeCardGameVMData.NormalTurn))
            .AddLabel("Status", nameof(HorseshoeCardGameVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Cards Left", false, nameof(HorseshoeCardGamePlayerItem.ObjectCount))
            .AddColumn("Tricks Won", false, nameof(HorseshoeCardGamePlayerItem.TricksWon))
            .AddColumn("Previous Score", false, nameof(HorseshoeCardGamePlayerItem.PreviousScore))
            .AddColumn("Total Score", false, nameof(HorseshoeCardGamePlayerItem.TotalScore));
        base.OnInitialized();
    }
}