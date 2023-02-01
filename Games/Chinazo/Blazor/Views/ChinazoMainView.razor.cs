namespace Chinazo.Blazor.Views;
public partial class ChinazoMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private ChinazoVMData? _vmData;
    private ChinazoGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<ChinazoVMData>();
        _gameContainer = aa1.Resolver.Resolve<ChinazoGameContainer>();
        _labels.AddLabel("Turn", nameof(ChinazoVMData.NormalTurn))
            .AddLabel("Status", nameof(ChinazoVMData.Status))
            .AddLabel("Other Turn", nameof(ChinazoVMData.OtherLabel))
            .AddLabel("Phase", nameof(ChinazoVMData.PhaseData));
        _scores.Clear();
        _scores.AddColumn("C Left", false, nameof(ChinazoPlayerItem.ObjectCount))
            .AddColumn("C Score", false, nameof(ChinazoPlayerItem.CurrentScore))
            .AddColumn("T Score", false, nameof(ChinazoPlayerItem.TotalScore));
        base.OnInitialized();
    }
    private ICustomCommand PassCommand => DataContext!.PassCommand!;
    private ICustomCommand TakeCommand => DataContext!.TakeCommand!;
    private ICustomCommand LayDownCommand => DataContext!.FirstSetsCommand!;
}