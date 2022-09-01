namespace RoundsCardGame.Blazor.Views;
public partial class RoundsCardGameMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private RoundsCardGameVMData? _vmData;
    private RoundsCardGameGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa.Resolver!.Resolve<RoundsCardGameVMData>();
        _gameContainer = aa.Resolver.Resolve<RoundsCardGameGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(RoundsCardGameVMData.NormalTurn))
           .AddLabel("Trump", nameof(RoundsCardGameVMData.TrumpSuit))
           .AddLabel("Status", nameof(RoundsCardGameVMData.Status));
        _scores.Clear();
        _scores.AddColumn("# In Hand", true, nameof(RoundsCardGamePlayerItem.ObjectCount))
            .AddColumn("Tricks Won", true, nameof(RoundsCardGamePlayerItem.TricksWon))
            .AddColumn("Rounds Won", true, nameof(RoundsCardGamePlayerItem.RoundsWon))
            .AddColumn("Points", true, nameof(RoundsCardGamePlayerItem.CurrentPoints))
            .AddColumn("Total Score", true, nameof(RoundsCardGamePlayerItem.TotalScore));
        base.OnInitialized();
    }
}