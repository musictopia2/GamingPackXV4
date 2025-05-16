namespace DominosMexicanTrain.Blazor.Views;
public partial class DominosMexicanTrainMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = [];
    private readonly BasicList<ScoreColumnModel> _scores = [];
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(DominosMexicanTrainVMData.NormalTurn))
            .AddLabel("Status", nameof(DominosMexicanTrainVMData.Status));
        _scores.AddColumn("Dominos Left", true, nameof(DominosMexicanTrainPlayerItem.ObjectCount))
               .AddColumn("Total Score", true, nameof(DominosMexicanTrainPlayerItem.TotalScore))
               .AddColumn("Previous Score", true, nameof(DominosMexicanTrainPlayerItem.PreviousScore))
               .AddColumn("# Previous", true, nameof(DominosMexicanTrainPlayerItem.PreviousLeft));
        base.OnInitialized();
    }
    public static MexicanDomino GetDomino
    {
        get
        {
            MexicanDomino output = new();
            output.IsUnknown = true;
            output.Deck = 1; //needed so the back can show up properly.
            return output;
        }
    }
    private ICustomCommand EndCommand => DataContext!.EndTurnCommand!;
    private ICustomCommand LongestCommand => DataContext!.LongestTrainCommand!;
}