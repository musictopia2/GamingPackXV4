namespace HuseHearts.Blazor.Views;
public partial class HuseHeartsMainView
{
    private readonly BasicList<LabelGridModel> _labels = [];
    private readonly BasicList<ScoreColumnModel> _scores = [];
    private HuseHeartsVMData? _vmData;
    private HuseHeartsGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = Resolver!.Resolve<HuseHeartsVMData>();
        _gameContainer = Resolver.Resolve<HuseHeartsGameContainer>();
        _labels.AddLabel("Turn", nameof(HuseHeartsVMData.NormalTurn))
            .AddLabel("Status", nameof(HuseHeartsVMData.Status))
            .AddLabel("Round", nameof(HuseHeartsVMData.RoundNumber));
        _scores.Clear();
        _scores.AddColumn("Cards Left", false, nameof(HuseHeartsPlayerItem.ObjectCount), numberOfDigits: 2)
            .AddColumn("Tricks Won", false, nameof(HuseHeartsPlayerItem.TricksWon), numberOfDigits: 2)
            .AddColumn("Current Score", false, nameof(HuseHeartsPlayerItem.CurrentScore), numberOfDigits: 3)
            .AddColumn("Previous Score", false, nameof(HuseHeartsPlayerItem.PreviousScore), numberOfDigits: 3)
            .AddColumn("Total Score", false, nameof(HuseHeartsPlayerItem.TotalScore), numberOfDigits: 3);
        base.OnInitialized();
    }
}