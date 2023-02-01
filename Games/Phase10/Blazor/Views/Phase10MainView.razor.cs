namespace Phase10.Blazor.Views;
public partial class Phase10MainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private Phase10VMData? _vmData;
    private Phase10GameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<Phase10VMData>();
        _gameContainer = aa1.Resolver.Resolve<Phase10GameContainer>();
        _labels.AddLabel("Turn", nameof(Phase10VMData.NormalTurn))
            .AddLabel("Status", nameof(Phase10VMData.Status))
            .AddLabel("Phase", nameof(Phase10VMData.CurrentPhase));
        _scores.Clear();
        _scores.AddColumn("Score", true, nameof(Phase10PlayerItem.TotalScore))
            .AddColumn("Cards Left", true, nameof(Phase10PlayerItem.ObjectCount))
            .AddColumn("Phase", true, nameof(Phase10PlayerItem.Phase))
            .AddColumn("Skipped", true, nameof(Phase10PlayerItem.MissNextTurn), category: EnumScoreSpecialCategory.TrueFalse)
            .AddColumn("Completed", true, nameof(Phase10PlayerItem.Completed), category: EnumScoreSpecialCategory.TrueFalse);
        base.OnInitialized();
    }
    private ICustomCommand CompleteCommand => DataContext!.CompletePhaseCommand!;
    private ICustomCommand SKipCommand => DataContext!.SkipPlayerCommand!;
    private bool NeedsToChooseSkip => _gameContainer!.SaveRoot.Skips;
}