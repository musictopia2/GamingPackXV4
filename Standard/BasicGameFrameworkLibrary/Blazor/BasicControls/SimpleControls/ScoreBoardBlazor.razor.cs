namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SimpleControls;
public partial class ScoreBoardBlazor<P>
    where P : class, IPlayerItem, IScoreBoard, new()
{
    [Parameter]
    public PlayerCollection<P>? Players { get; set; }
    [Parameter]
    public BasicList<ScoreColumnModel> Columns { get; set; } = new();
    [Parameter]
    public string Height { get; set; } = "";
    [Parameter]
    public string Width { get; set; } = "";
    [Parameter]
    public bool UseAbbreviationForTrueFalse { get; set; }
    private string TextToDisplay(P player, ScoreColumnModel column)
    {
        return player.TextToDisplay(column, UseAbbreviationForTrueFalse);
    }
}