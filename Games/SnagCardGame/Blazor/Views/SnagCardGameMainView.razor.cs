namespace SnagCardGame.Blazor.Views;
public partial class SnagCardGameMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private SnagCardGameVMData? _vmData;
    private SnagCardGameGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa.Resolver!.Resolve<SnagCardGameVMData>();
        _gameContainer = aa.Resolver.Resolve<SnagCardGameGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(SnagCardGameVMData.NormalTurn))
           .AddLabel("Status", nameof(SnagCardGameVMData.Status))
           .AddLabel("Instructions", nameof(SnagCardGameVMData.Instructions));
        _scores.Clear();
        _scores.AddColumn("Cards Won", true, nameof(SnagCardGamePlayerItem.CardsWon))
            .AddColumn("Current Points", true, nameof(SnagCardGamePlayerItem.CurrentPoints))
            .AddColumn("Total Points", true, nameof(SnagCardGamePlayerItem.TotalPoints));
        base.OnInitialized();
    }
}