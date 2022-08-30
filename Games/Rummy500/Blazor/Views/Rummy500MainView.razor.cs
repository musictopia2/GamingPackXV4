namespace Rummy500.Blazor.Views;
public partial class Rummy500MainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private Rummy500VMData? _vmData;
    private Rummy500GameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa.Resolver!.Resolve<Rummy500VMData>();
        _gameContainer = aa.Resolver.Resolve<Rummy500GameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(Rummy500VMData.NormalTurn))
           .AddLabel("Status", nameof(Rummy500VMData.Status));
        _scores.Clear();
        _scores.AddColumn("Cards Left", false, nameof(Rummy500PlayerItem.ObjectCount))
            .AddColumn("Points Played", false, nameof(Rummy500PlayerItem.PointsPlayed))
            .AddColumn("Cards Played", false, nameof(Rummy500PlayerItem.CardsPlayed))
            .AddColumn("Score Current", false, nameof(Rummy500PlayerItem.CurrentScore))
            .AddColumn("Score Total", false, nameof(Rummy500PlayerItem.TotalScore));
        base.OnInitialized();
    }
    private ICustomCommand DiscardCommand => DataContext!.DiscardCurrentCommand!;
    private ICustomCommand CreateSetCommand => DataContext!.CreateSetCommand!;
}