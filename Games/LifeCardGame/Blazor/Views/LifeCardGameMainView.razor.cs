namespace LifeCardGame.Blazor.Views;
public partial class LifeCardGameMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private LifeCardGameVMData? _vmData;
    private LifeCardGameGameContainer? _gameContainer;
    private static string GetColums => bb1.RepeatAuto(2);
    private BasicList<LifeCardGamePlayerItem> _players = new();
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<LifeCardGameVMData>();
        _gameContainer = aa1.Resolver.Resolve<LifeCardGameGameContainer>();
        _players = _gameContainer.PlayerList!.GetAllPlayersStartingWithSelf();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(LifeCardGameVMData.NormalTurn))
            .AddLabel("Status", nameof(LifeCardGameVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Cards Left", true, nameof(LifeCardGamePlayerItem.ObjectCount))
            .AddColumn("Points", true, nameof(LifeCardGamePlayerItem.Points));
        base.OnInitialized();
    }
    private ICustomCommand YearsCommand => DataContext!.YearsPassedCommand!;
    private ICustomCommand PlayCardCommand => DataContext!.PlayCardCommand!;
}