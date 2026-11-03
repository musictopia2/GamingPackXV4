namespace FiveCrowns.Blazor.Views;
public partial class FiveCrownsMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = [];
    private readonly BasicList<ScoreColumnModel> _scores = [];
    private FiveCrownsVMData? _vmData;
    private FiveCrownsGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = Resolver!.Resolve<FiveCrownsVMData>();
        _gameContainer = Resolver.Resolve<FiveCrownsGameContainer>();
        _labels.AddLabel("Turn", nameof(FiveCrownsVMData.NormalTurn))
             .AddLabel("Status", nameof(FiveCrownsVMData.Status))
             .AddLabel("Up To", nameof(FiveCrownsVMData.UpTo));
        _scores.Clear();
        _scores.AddColumn("Cards Left", true, nameof(FiveCrownsPlayerItem.ObjectCount))
            .AddColumn("Current Score", true, nameof(FiveCrownsPlayerItem.CurrentScore))
            .AddColumn("Total Score", true, nameof(FiveCrownsPlayerItem.TotalScore));
        base.OnInitialized();
    }
    private ICustomCommand LayDownCommand => DataContext!.LayDownSetsCommand!;
    private ICustomCommand BackCommand => DataContext!.BackCommand!;
}