namespace GoFish.Blazor.Views;
public partial class GoFishMainView
{
    private readonly BasicList<LabelGridModel> _labels = [];
    private readonly BasicList<ScoreColumnModel> _scores = [];
    private GoFishVMData? _vmData;
    private GoFishGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = Resolver!.Resolve<GoFishVMData>();
        _gameContainer = Resolver.Resolve<GoFishGameContainer>();
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