namespace LifeCardGame.Blazor.Views;
public partial class LifeCardGameMainView
{
    private readonly BasicList<LabelGridModel> _labels = [];
    private readonly BasicList<ScoreColumnModel> _scores = [];
    private LifeCardGameVMData? _vmData;
    private LifeCardGameGameContainer? _gameContainer;
    private static string GetColums => bb1.RepeatAuto(2);
    private BasicList<LifeCardGamePlayerItem> _players = [];
    protected override void OnInitialized()
    {
        _vmData = Resolver!.Resolve<LifeCardGameVMData>();
        _gameContainer = Resolver.Resolve<LifeCardGameGameContainer>();
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