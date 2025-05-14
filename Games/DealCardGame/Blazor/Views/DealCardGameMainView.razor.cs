namespace DealCardGame.Blazor.Views;
public partial class DealCardGameMainView
{

    private bool _isTesting = false;

    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = [];
    private readonly BasicList<ScoreColumnModel> _scores = [];
    private DealCardGameVMData? _vmData;
    private DealCardGameGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<DealCardGameVMData>();
        _gameContainer = aa1.Resolver.Resolve<DealCardGameGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(DealCardGameVMData.NormalTurn))
            .AddLabel("Status", nameof(DealCardGameVMData.Status))
            .AddLabel("Plays Remaining", nameof(DealCardGameVMData.PlaysRemaining));
        _scores.Clear();
        _scores.AddColumn("Cards Left", true, nameof(DealCardGamePlayerItem.ObjectCount))
            .AddColumn("Money", true, nameof(DealCardGamePlayerItem.Money), category: EnumScoreSpecialCategory.Currency)
            .AddColumn("Monopolies", true, nameof(DealCardGamePlayerItem.Monopolies))
            ; //cards left is common.  can be anything you need.
        base.OnInitialized();
    }
    private BasicList<DealCardGamePlayerItem> GetPlayers()
    {
        var output = _gameContainer!.PlayerList!.GetAllPlayersStartingWithSelf();
        return output;
    }
    private static string Rows => $"16vh {gg1.RepeatMinimum(1)}";
    private BasicGameCommand PlayCommand => DataContext!.PlayCommand!;
    private BasicGameCommand EndTurnCommand => DataContext!.EndTurnCommand!;
    private BasicGameCommand BankCommand => DataContext!.BankCommand!;
    private BasicGameCommand SetChosenCommand => DataContext!.SetChosenCommand!;
    private BasicGameCommand ResumeCommand => DataContext!.ResumeCommand!;
    private BasicGameCommand ChoosePlayerCommand => DataContext!.ChoosePlayerCommand!;
    private BasicGameCommand StartOrganizingCommand => DataContext!.StartOrganizingCommand!;
    private string YourDisplayText => $"{DataContext!.VMData.TradeDisplay!.WhoPlayerName} receives";
    private string OpponentDisplayText => $"{DataContext!.VMData.TradeDisplay!.TradePlayerName} receives";
}