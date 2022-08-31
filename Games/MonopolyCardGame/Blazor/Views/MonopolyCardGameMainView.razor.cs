namespace MonopolyCardGame.Blazor.Views;
public partial class MonopolyCardGameMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private MonopolyCardGameVMData? _vmData;
    private MonopolyCardGameGameContainer? _gameContainer;
    private BasicList<MonopolyCardGamePlayerItem> _players = new();
    protected override void OnInitialized()
    {
        _vmData = aa.Resolver!.Resolve<MonopolyCardGameVMData>();
        _gameContainer = aa.Resolver.Resolve<MonopolyCardGameGameContainer>();
        _players = _gameContainer.PlayerList!.GetAllPlayersStartingWithSelf();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(MonopolyCardGameVMData.NormalTurn))
           .AddLabel("Status", nameof(MonopolyCardGameVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Cards Left", true, nameof(MonopolyCardGamePlayerItem.ObjectCount))
            .AddColumn("Previous Money", true, nameof(MonopolyCardGamePlayerItem.PreviousMoney), category: EnumScoreSpecialCategory.Currency)
            .AddColumn("Total Money", true, nameof(MonopolyCardGamePlayerItem.TotalMoney), category: EnumScoreSpecialCategory.Currency);
        base.OnInitialized();
    }
    private bool IsSelf => _gameContainer!.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self;
    private ICustomCommand ResumeCommand => DataContext!.ResumeCommand!;
    private ICustomCommand GoOutCommand => DataContext!.GoOutCommand!;
}