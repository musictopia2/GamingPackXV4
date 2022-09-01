namespace HuseHearts.Blazor.Views;
public partial class HuseHeartsMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private HuseHeartsVMData? _vmData;
    private HuseHeartsGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa.Resolver!.Resolve<HuseHeartsVMData>();
        _gameContainer = aa.Resolver.Resolve<HuseHeartsGameContainer>();
        _labels.AddLabel("Turn", nameof(HuseHeartsVMData.NormalTurn))
            .AddLabel("Status", nameof(HuseHeartsVMData.Status))
            .AddLabel("Round", nameof(HuseHeartsVMData.RoundNumber));
        _scores.Clear();
        _scores.AddColumn("Cards Left", false, nameof(HuseHeartsPlayerItem.ObjectCount))
            .AddColumn("Tricks Won", false, nameof(HuseHeartsPlayerItem.TricksWon))
            .AddColumn("Current Score", false, nameof(HuseHeartsPlayerItem.CurrentScore))
            .AddColumn("Previous Score", false, nameof(HuseHeartsPlayerItem.PreviousScore))
            .AddColumn("Total Score", false, nameof(HuseHeartsPlayerItem.TotalScore));
        base.OnInitialized();
    }
}