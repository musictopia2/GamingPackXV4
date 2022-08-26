namespace DominoBonesMultiplayerGames.Blazor.Views;
public partial class DominoBonesMultiplayerGamesMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(DominoBonesMultiplayerGamesVMData.NormalTurn))
            .AddLabel("Status", nameof(DominoBonesMultiplayerGamesVMData.Status));
        base.OnInitialized();
    }
    public SimpleDominoInfo GetDomino
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