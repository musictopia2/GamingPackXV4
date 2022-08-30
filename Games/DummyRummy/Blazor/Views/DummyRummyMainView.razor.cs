namespace DummyRummy.Blazor.Views;
public partial class DummyRummyMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private DummyRummyVMData? _vmData;
    private DummyRummyGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa.Resolver!.Resolve<DummyRummyVMData>();
        _gameContainer = aa.Resolver.Resolve<DummyRummyGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(DummyRummyVMData.NormalTurn))
           .AddLabel("Status", nameof(DummyRummyVMData.Status))
           .AddLabel("Up To", nameof(DummyRummyVMData.UpTo));
        _scores.Clear();
        _scores.AddColumn("Cards Left", true, nameof(DummyRummyPlayerItem.ObjectCount))
            .AddColumn("Current Score", true, nameof(DummyRummyPlayerItem.CurrentScore))
            .AddColumn("Total Score", true, nameof(DummyRummyPlayerItem.TotalScore));
        base.OnInitialized();
    }
    private ICustomCommand LayDownCommand => DataContext!.LayDownSetsCommand!;
    private ICustomCommand BackCommand => DataContext!.BackCommand!;
}