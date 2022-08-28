namespace ThinkTwice.Blazor.Views;
public partial class ThinkTwiceMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private readonly string _diceHeight = "15vh";
    private ThinkTwiceGameContainer? _gameContainer;
    private CategoriesDice? _categories;
    private Multiplier? _multiplier;
    protected override void OnInitialized()
    {
        _categories = aa.Resolver!.Resolve<CategoriesDice>();
        _multiplier = aa.Resolver.Resolve<Multiplier>();
        _gameContainer = aa.Resolver.Resolve<ThinkTwiceGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(ThinkTwiceVMData.NormalTurn))
            .AddLabel("Roll", nameof(ThinkTwiceVMData.RollNumber))
            .AddLabel("Category", nameof(ThinkTwiceVMData.CategoryChosen))
            .AddLabel("Score", nameof(ThinkTwiceVMData.Score))
            .AddLabel("Status", nameof(ThinkTwiceVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Score Round", true, nameof(ThinkTwicePlayerItem.ScoreRound))
            .AddColumn("Score Game", true, nameof(ThinkTwicePlayerItem.ScoreGame));
        base.OnInitialized();
    }
    private ICustomCommand EndCommand => DataContext!.EndTurnCommand!;
    private ICustomCommand RollCommand => DataContext!.RollDiceCommand!;
    private ICustomCommand MultiCommand => DataContext!.RollMultCommand!;
    private async Task CategoryClickedAsync()
    {
        if (_categories!.Visible == false)
        {
            return;
        }
        await _gameContainer!.CategoryClicked!.Invoke();
    }
}