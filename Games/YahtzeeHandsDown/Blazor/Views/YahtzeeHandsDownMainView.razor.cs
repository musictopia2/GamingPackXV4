namespace YahtzeeHandsDown.Blazor.Views;
public partial class YahtzeeHandsDownMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private YahtzeeHandsDownVMData? _vmData;
    private YahtzeeHandsDownGameContainer? _gameContainer;
    private static string GetColumns => bb1.RepeatAuto(2);
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<YahtzeeHandsDownVMData>();
        _gameContainer = aa1.Resolver.Resolve<YahtzeeHandsDownGameContainer>();
        _labels.AddLabel("Turn", nameof(YahtzeeHandsDownVMData.NormalTurn))
             .AddLabel("Status", nameof(YahtzeeHandsDownVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Cards Left", true, nameof(YahtzeeHandsDownPlayerItem.ObjectCount))
            .AddColumn("Total Score", true, nameof(YahtzeeHandsDownPlayerItem.TotalScore))
            .AddColumn("Won Last Round", true, nameof(YahtzeeHandsDownPlayerItem.WonLastRound))
            .AddColumn("Score Round", true, nameof(YahtzeeHandsDownPlayerItem.ScoreRound));
        base.OnInitialized();
    }
    private ICustomCommand OutCommand => DataContext!.GoOutCommand!;
    private ICustomCommand EndCommand => DataContext!.EndTurnCommand!;
}