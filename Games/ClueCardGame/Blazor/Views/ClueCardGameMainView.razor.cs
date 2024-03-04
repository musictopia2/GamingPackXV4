namespace ClueCardGame.Blazor.Views;
public partial class ClueCardGameMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = [];
    private ClueCardGameVMData? _vmData;
    private ClueCardGameGameContainer? _gameContainer;
    private PlayerCollection<ClueCardGamePlayerItem> _players = [];
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<ClueCardGameVMData>();
        _gameContainer = aa1.Resolver.Resolve<ClueCardGameGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(ClueCardGameVMData.NormalTurn))
            .AddLabel("Status", nameof(ClueCardGameVMData.Status))
            .AddLabel("First Name", nameof(ClueCardGameVMData.FirstName))
            .AddLabel("Second Name ", nameof(ClueCardGameVMData.SecondName))
        ;
        _players = _gameContainer.PlayerList!;
        base.OnInitialized();
    }
    private BasicGameCommand EndTurnCommand => DataContext!.EndTurnCommand!;
    private string GetStyle(ClueCardGamePlayerItem player) => _gameContainer!.SaveRoot.WhoGaveClue == player.NickName ? "color: lime;" : "";
}