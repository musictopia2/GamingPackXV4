namespace GolfCardGame.Blazor.Views;
public partial class GolfCardGameMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    [CascadingParameter]
    public GolfCardGameVMData? VMData { get; set; }
    private GolfCardGameGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _gameContainer = aa.Resolver!.Resolve<GolfCardGameGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(GolfCardGameVMData.NormalTurn))
           .AddLabel("Status", nameof(GolfCardGameVMData.Status))
           .AddLabel("Round", nameof(GolfCardGameVMData.Round))
           .AddLabel("Instructions", nameof(GolfCardGameVMData.Instructions));
        _scores.Clear();
        _scores.AddColumn("Knocked", false, nameof(GolfCardGamePlayerItem.Knocked), category: EnumScoreSpecialCategory.TrueFalse)
            .AddColumn("Changed 1", false, nameof(GolfCardGamePlayerItem.FirstChanged), category: EnumScoreSpecialCategory.TrueFalse)
            .AddColumn("Changed 2", false, nameof(GolfCardGamePlayerItem.SecondChanged), category: EnumScoreSpecialCategory.TrueFalse)
            .AddColumn("Previous Score", false, nameof(GolfCardGamePlayerItem.PreviousScore))
            .AddColumn("Total Score", false, nameof(GolfCardGamePlayerItem.TotalScore));
        base.OnInitialized();
    }
    private ICustomCommand KnockCommand => DataContext!.KnockCommand!;
}