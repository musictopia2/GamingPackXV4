namespace Hearts.Blazor.Views;
public partial class HeartsMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private HeartsVMData? _vmData;
    private HeartsGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<HeartsVMData>();
        _gameContainer = aa1.Resolver.Resolve<HeartsGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(HeartsVMData.NormalTurn))
            .AddLabel("Status", nameof(HeartsVMData.Status))
            .AddLabel("Round", nameof(HeartsVMData.RoundNumber))
            .AddLabel("Pass", nameof(HeartsVMData.PassedPlayer))
            ;

        _scores.Clear();
        _scores.AddColumn("Cards Left", false, nameof(HeartsPlayerItem.ObjectCount))
            .AddColumn("Tricks Won", false, nameof(HeartsPlayerItem.TricksWon))
            .AddColumn("Current Score", false, nameof(HeartsPlayerItem.CurrentScore))
            .AddColumn("Previous Score", false, nameof(HeartsPlayerItem.PreviousScore))
            .AddColumn("Total Score", false, nameof(HeartsPlayerItem.TotalScore));
        ; //cards left is common.  can be anything you need.
        base.OnInitialized();
    }
    private ICustomCommand PassCommand => DataContext!.PassCommand!;
    private ICustomCommand MoonCommand => DataContext!.MoonCommand!;
}