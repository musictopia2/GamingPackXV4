namespace GoFish.Blazor.Views;
public partial class GoFishMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private GoFishVMData? _vmData;
    private GoFishGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<GoFishVMData>();
        _gameContainer = aa1.Resolver.Resolve<GoFishGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(GoFishVMData.NormalTurn))
            .AddLabel("Status", nameof(GoFishVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Cards Left", true, nameof(GoFishPlayerItem.ObjectCount))
            .AddColumn("Pairs", true, nameof(GoFishPlayerItem.Pairs));
        base.OnInitialized();
    }
    private ICustomCommand EndCommand => DataContext!.EndTurnCommand!;
}