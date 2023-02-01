namespace Cribbage.Blazor.Views;
public partial class CribbageMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private CribbageVMData? _vmData;
    private CribbageGameContainer? _gameContainer;
    private readonly BasicList<LabelGridModel> _counts = new();
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<CribbageVMData>();
        _gameContainer = aa1.Resolver.Resolve<CribbageGameContainer>();
        _labels.AddLabel("Turn", nameof(CribbageVMData.NormalTurn))
            .AddLabel("Status", nameof(CribbageVMData.Status))
            .AddLabel("Dealer", nameof(CribbageVMData.Dealer));
        _counts.Clear();
        _counts.AddLabel("Count", nameof(CribbageVMData.TotalCount));
        _scores.Clear();
        _scores.AddColumn("Cards Left", false, nameof(CribbagePlayerItem.ObjectCount))
            .AddColumn("Is Skunk Hole", false, nameof(CribbagePlayerItem.IsSkunk), category: EnumScoreSpecialCategory.TrueFalse)
            .AddColumn("First Position", false, nameof(CribbagePlayerItem.FirstPosition))
            .AddColumn("Second Position", false, nameof(CribbagePlayerItem.SecondPosition))
            .AddColumn("Score Round", false, nameof(CribbagePlayerItem.ScoreRound))
            .AddColumn("Score Game", false, nameof(CribbagePlayerItem.TotalScore));
        base.OnInitialized();
    }
    private ICustomCommand ContinueCommand => DataContext!.ContinueCommand!;
    private ICustomCommand CribCommand => DataContext!.CribCommand!;
    private ICustomCommand PlayCommand => DataContext!.PlayCommand!;
    private static string Columns => bb1.RepeatAuto(2);
}