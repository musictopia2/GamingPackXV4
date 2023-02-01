namespace Racko.Blazor.Views;
public partial class RackoMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private RackoVMData? _vmData;
    private RackoGameContainer? _gameContainer;
    private static string GetColumns => bb1.RepeatAuto(2);
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<RackoVMData>();
        _gameContainer = aa1.Resolver.Resolve<RackoGameContainer>();
        _labels.AddLabel("Turn", nameof(RackoVMData.NormalTurn))
           .AddLabel("Status", nameof(RackoVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Cards Left", true, nameof(RackoPlayerItem.ObjectCount))
            .AddColumn("Score Round", true, nameof(RackoPlayerItem.ScoreRound))
            .AddColumn("Score Game", true, nameof(RackoPlayerItem.TotalScore));
        int x;
        for (x = 1; x <= 10; x++)
        {
            _scores.AddColumn("Section" + x, false, "Value" + x, nameof(RackoPlayerItem.CanShowValues));// 2 bindings.
        }
        base.OnInitialized();
    }
    private ICustomCommand DiscardCommand => DataContext!.DiscardCurrentCommand!;
    private ICustomCommand RackoCommand => DataContext!.RackoCommand!;
}