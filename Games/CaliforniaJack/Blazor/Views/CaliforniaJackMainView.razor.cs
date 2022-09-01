namespace CaliforniaJack.Blazor.Views;
public partial class CaliforniaJackMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private CaliforniaJackVMData? _vmData;
    private CaliforniaJackGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa.Resolver!.Resolve<CaliforniaJackVMData>();
        _gameContainer = aa.Resolver.Resolve<CaliforniaJackGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(CaliforniaJackVMData.NormalTurn))
             .AddLabel("Trump", nameof(CaliforniaJackVMData.TrumpSuit))
             .AddLabel("Status", nameof(CaliforniaJackVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Cards Left", true, nameof(CaliforniaJackPlayerItem.ObjectCount))
            .AddColumn("Tricks Won", true, nameof(CaliforniaJackPlayerItem.TricksWon))
            .AddColumn("Points", true, nameof(CaliforniaJackPlayerItem.Points))
            .AddColumn("Total Score", true, nameof(CaliforniaJackPlayerItem.TotalScore));
        base.OnInitialized();
    }
}