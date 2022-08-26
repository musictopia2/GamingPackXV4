namespace TileRummy.Blazor.Views;
public partial class TileRummyMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(TileRummyVMData.NormalTurn))
            .AddLabel("Status", nameof(TileRummyVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Tiles Left", true, nameof(TileRummyPlayerItem.ObjectCount))
            .AddColumn("Score", true, nameof(TileRummyPlayerItem.Score));
        base.OnInitialized();
    }
    public static TileInfo GetTile
    {
        get
        {
            TileInfo output = new();
            output.IsUnknown = true;
            output.Deck = 1; //needed so the back can show up properly.
            return output;
        }
    }
    private ICustomCommand FirstSetCommand => DataContext!.CreateFirstSetsCommand!;
    private ICustomCommand CreateNewSetCommand => DataContext!.CreateNewSetCommand!;
    private ICustomCommand ResetCommand => DataContext!.UndoMoveCommand!;
    private ICustomCommand EndCommand => DataContext!.EndTurnCommand!;
}