using BasicGameFrameworkLibrary.Core.ScoreBoardClassesCP; //not common enough.
namespace BasicGameFrameworkLibrary.Blazor.Views;
public partial class YahtzeeMainView<D>
    where D : SimpleDice, new()
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private int _bottomDescriptionWidth;
    private ScoreContainer? _scoreContainer;
    protected override void OnInitialized()
    {
        _scoreContainer = null;
        _labels.Clear();
        IYahtzeeStyle yahtzeeStyle = Resolver!.Resolve<IYahtzeeStyle>();
        _bottomDescriptionWidth = yahtzeeStyle.BottomDescriptionWidth;
        DataContext!.CommandContainer.AddAction(ShowChange);
        _labels.AddLabel("Turn", nameof(YahtzeeVMData<D>.NormalTurn))
            .AddLabel("Roll", nameof(YahtzeeVMData<D>.RollNumber))
            .AddLabel("Status", nameof(YahtzeeVMData<D>.Status))
            .AddLabel("Turn #", nameof(YahtzeeVMData<D>.Round));
        _scores.Clear();
        _scores.AddColumn("Points", false, nameof(YahtzeePlayerItem<D>.Points));
        base.OnInitialized();
    }
    private ICustomCommand RollCommand => DataContext!.RollDiceCommand!;
    private static ScoreContainer GetContainer()
    {
        return Resolver!.Resolve<ScoreContainer>();
    }
}