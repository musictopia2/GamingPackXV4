namespace A8RoundRummy.Blazor.Views;
public partial class A8RoundRummyMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private A8RoundRummyVMData? _vmData;
    private A8RoundRummyGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa.Resolver!.Resolve<A8RoundRummyVMData>();
        _gameContainer = aa.Resolver.Resolve<A8RoundRummyGameContainer>();
        _labels.AddLabel("Turn", nameof(A8RoundRummyVMData.NormalTurn))
            .AddLabel("Status", nameof(A8RoundRummyVMData.Status))
            .AddLabel("Next", nameof(A8RoundRummyVMData.NextTurn));
        _scores.Clear();
        _scores.AddColumn("Cards Left", true, nameof(A8RoundRummyPlayerItem.ObjectCount))
            .AddColumn("Total Score", true, nameof(A8RoundRummyPlayerItem.TotalScore));
        base.OnInitialized();
    }
    private ICustomCommand GoOutCommand => DataContext!.GoOutCommand!;
    private static string GetColumns => bb.RepeatAuto(2);
}