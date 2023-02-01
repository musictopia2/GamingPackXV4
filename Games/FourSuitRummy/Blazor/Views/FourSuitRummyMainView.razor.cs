namespace FourSuitRummy.Blazor.Views;
public partial class FourSuitRummyMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private FourSuitRummyVMData? _vmData;
    private FourSuitRummyGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<FourSuitRummyVMData>();
        _gameContainer = aa1.Resolver.Resolve<FourSuitRummyGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(FourSuitRummyVMData.NormalTurn))
            .AddLabel("Status", nameof(FourSuitRummyVMData.Status));
        _scores.AddColumn("Cards Left", true, nameof(FourSuitRummyPlayerItem.ObjectCount))
            .AddColumn("Total Score", true, nameof(FourSuitRummyPlayerItem.TotalScore));
        base.OnInitialized();
    }
    private ICustomCommand PlayCommand => DataContext!.PlaySetsCommand!;
    private FourSuitRummyPlayerItem GetSelf => _gameContainer!.PlayerList!.GetSelf();
    private FourSuitRummyPlayerItem GetOpponent => _gameContainer!.PlayerList!.GetOnlyOpponent();
}