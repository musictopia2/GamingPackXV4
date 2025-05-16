namespace DominosRegular.Blazor.Views;
public partial class DominosRegularMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    [CascadingParameter]
    public int TargetHeight { get; set; } //i like the idea of cascading values here.

    private readonly BasicList<LabelGridModel> _labels = [];
    private readonly BasicList<ScoreColumnModel> _scores = [];
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(DominosRegularVMData.NormalTurn))
            .AddLabel("Status", nameof(DominosRegularVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Total Score", true, nameof(DominosRegularPlayerItem.TotalScore))
                .AddColumn("Dominos Left", true, nameof(DominosRegularPlayerItem.ObjectCount));
        base.OnInitialized();
    }
    public static SimpleDominoInfo GetDomino
    {
        get
        {
            SimpleDominoInfo output = new();
            output.IsUnknown = true;
            output.Deck = 1; //needed so the back can show up properly.
            return output;
        }
    }
    private ICustomCommand EndTurnCommand => DataContext!.EndTurnCommand!;
    private string GetTargetString => TargetHeight.HeightString();
}