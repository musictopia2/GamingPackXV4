namespace GalaxyCardGame.Blazor.Views;
public partial class GalaxyCardGameMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private GalaxyCardGameVMData? _vmData;
    private GalaxyCardGameGameContainer? _gameContainer;
    private BasicList<GalaxyCardGamePlayerItem> _players = new();
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<GalaxyCardGameVMData>();
        _gameContainer = aa1.Resolver.Resolve<GalaxyCardGameGameContainer>();
        _players = _gameContainer.PlayerList!.GetAllPlayersStartingWithSelf();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(GalaxyCardGameVMData.NormalTurn))
            .AddLabel("Trump", nameof(GalaxyCardGameVMData.TrumpSuit))
            .AddLabel("Status", nameof(GalaxyCardGameVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Cards Left", true, nameof(GalaxyCardGamePlayerItem.ObjectCount))
            .AddColumn("Tricks Won", true, nameof(GalaxyCardGamePlayerItem.TricksWon));
        base.OnInitialized();
    }
    private ICustomCommand EndCommand => DataContext!.EndTurnCommand!;
    private ICustomCommand MoonCommand => DataContext!.MoonCommand!;
}