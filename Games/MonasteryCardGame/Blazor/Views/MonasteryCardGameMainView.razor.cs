namespace MonasteryCardGame.Blazor.Views;
public partial class MonasteryCardGameMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private MonasteryCardGameVMData? _vmData;
    private MonasteryCardGameGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<MonasteryCardGameVMData>();
        _gameContainer = aa1.Resolver.Resolve<MonasteryCardGameGameContainer>();
        _labels.AddLabel("Turn", nameof(MonasteryCardGameVMData.NormalTurn))
             .AddLabel("Status", nameof(MonasteryCardGameVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Cards", false, nameof(MonasteryCardGamePlayerItem.ObjectCount))
            .AddColumn("Done", false, nameof(MonasteryCardGamePlayerItem.FinishedCurrentMission), category: EnumScoreSpecialCategory.TrueFalse);
        int x;
        for (x = 1; x <= 9; x++)
        {
            _scores.AddColumn($"M{x}", false, $"Mission{x}Completed", category: EnumScoreSpecialCategory.TrueFalse);
        }
        base.OnInitialized();
    }
}