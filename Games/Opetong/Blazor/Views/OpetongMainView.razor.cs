namespace Opetong.Blazor.Views;
public partial class OpetongMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private OpetongVMData? _vmData;
    private OpetongGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa.Resolver!.Resolve<OpetongVMData>();
        _gameContainer = aa.Resolver.Resolve<OpetongGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(OpetongVMData.NormalTurn))
           .AddLabel("Status", nameof(OpetongVMData.Status))
           .AddLabel("Instructions", nameof(OpetongVMData.Instructions));
        _scores.Clear();
        _scores.AddColumn("Cards Left", true, nameof(OpetongPlayerItem.ObjectCount))
            .AddColumn("Sets Played", true, nameof(OpetongPlayerItem.SetsPlayed))
            .AddColumn("Score", true, nameof(OpetongPlayerItem.TotalScore));
        base.OnInitialized();
    }
    private ICustomCommand LayCommand => DataContext!.PlaySetCommand!;
}