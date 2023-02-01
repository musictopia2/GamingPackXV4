namespace CousinRummy.Blazor.Views;
public partial class CousinRummyMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private CousinRummyVMData? _vmData;
    private CousinRummyGameContainer? _gameContainer;
    private static string GetFirstRows => bb1.RepeatAuto(3);
    private static string GetSecondColumns => bb1.RepeatAuto(3);
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<CousinRummyVMData>();
        _gameContainer = aa1.Resolver.Resolve<CousinRummyGameContainer>();
        _labels.Clear();
        _labels.AddLabel("N.Turn", nameof(CousinRummyVMData.NormalTurn)) //try to have no spaces to stop the bug of going to next line as well
            .AddLabel("O.Turn", nameof(CousinRummyVMData.OtherLabel))
            .AddLabel("Phase", nameof(CousinRummyVMData.PhaseData));
        _scores.Clear();
        _scores.AddColumn("Cards Left", false, nameof(CousinRummyPlayerItem.ObjectCount))
            .AddColumn("Tokens Left", false, nameof(CousinRummyPlayerItem.TokensLeft))
            .AddColumn("Current Score", false, nameof(CousinRummyPlayerItem.CurrentScore))
            .AddColumn("Total Score", false, nameof(CousinRummyPlayerItem.TotalScore));
        base.OnInitialized();
    }
    private ICustomCommand InitCommand => DataContext!.FirstSetsCommand!;
    private ICustomCommand OtherCommand => DataContext!.OtherSetsCommand!;
    private ICustomCommand BuyCommand => DataContext!.BuyCommand!;
    private ICustomCommand PassCommand => DataContext!.PassCommand!;
}