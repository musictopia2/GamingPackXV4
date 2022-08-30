namespace BladesOfSteel.Blazor.Views;
public partial class BladesOfSteelMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    [CascadingParameter]
    public BladesOfSteelVMData? VMData { get; set; }
    private BladesOfSteelGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _gameContainer = aa.Resolver!.Resolve<BladesOfSteelGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Main Turn", nameof(BladesOfSteelVMData.NormalTurn))
           .AddLabel("Other Turn", nameof(BladesOfSteelVMData.OtherPlayer))
           .AddLabel("Status", nameof(BladesOfSteelVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Cards Left", true, nameof(BladesOfSteelPlayerItem.ObjectCount))
            .AddColumn("Score", true, nameof(BladesOfSteelPlayerItem.Score));
        base.OnInitialized();
    }
    private static string GetColumns()
    {
        return $"{gg.Auto} {gg.Auto} {gg.Auto}";
    }
    private static string GetRows()
    {
        return $"{gg.Auto} {gg.Auto}";
    }
    private ICustomCommand EndCommand => DataContext?.EndTurnCommand!;
    private ICustomCommand PassCommand => DataContext?.PassCommand!;
}