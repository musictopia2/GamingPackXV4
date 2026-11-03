namespace Hearts.Blazor.Views;
public partial class HeartsMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = [];
    private readonly BasicList<ScoreColumnModel> _scores = [];
    private HeartsVMData? _vmData;
    private HeartsGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = Resolver!.Resolve<HeartsVMData>();
        _gameContainer = Resolver.Resolve<HeartsGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(HeartsVMData.NormalTurn))
            .AddLabel("Status", nameof(HeartsVMData.Status))
            .AddLabel("Round", nameof(HeartsVMData.RoundNumber))
            .AddLabel("Pass", nameof(HeartsVMData.PassedPlayer));
        _scores.Clear();
        _scores.AddColumn("Cards Left", false, nameof(HeartsPlayerItem.ObjectCount), numberOfDigits: 2)
            .AddColumn("Tricks Won", false, nameof(HeartsPlayerItem.TricksWon), numberOfDigits: 2)
            .AddColumn("Current Score", false, nameof(HeartsPlayerItem.CurrentScore), numberOfDigits: 4)
            .AddColumn("Previous Score", false, nameof(HeartsPlayerItem.PreviousScore), numberOfDigits: 4)
            .AddColumn("Total Score", false, nameof(HeartsPlayerItem.TotalScore), numberOfDigits: 4);
        ; //cards left is common.  can be anything you need.
        base.OnInitialized();
    }
    private ICustomCommand PassCommand => DataContext!.PassCommand!;
    private ICustomCommand MoonCommand => DataContext!.MoonCommand!;
}