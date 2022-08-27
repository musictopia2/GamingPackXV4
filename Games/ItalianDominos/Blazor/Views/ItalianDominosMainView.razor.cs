namespace ItalianDominos.Blazor.Views;
public partial class ItalianDominosMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(ItalianDominosVMData.NormalTurn))
           .AddLabel("Status", nameof(ItalianDominosVMData.Status))
           .AddLabel("Up To", nameof(ItalianDominosVMData.UpTo))
           .AddLabel("Next #", nameof(ItalianDominosVMData.NextNumber));
        _scores.Clear();
        _scores.AddColumn("Total Score", true, nameof(ItalianDominosPlayerItem.TotalScore))
            .AddColumn("Dominos Left", true, nameof(ItalianDominosPlayerItem.ObjectCount))
            .AddColumn("Drew Yet", true, nameof(ItalianDominosPlayerItem.DrewYet), category: EnumScoreSpecialCategory.TrueFalse); //does not do it based on column
        base.OnInitialized();
    }
    private ICustomCommand PlayCommand => DataContext!.PlayCommand!;
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
}