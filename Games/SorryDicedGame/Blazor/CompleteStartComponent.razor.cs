namespace SorryDicedGame.Blazor;
public partial class CompleteStartComponent
{
    [Parameter]
    public string ImageHeight { get; set; } = "";
    [Parameter]
    public PlayerCollection<SorryDicedGamePlayerItem> Players { get; set; } = [];
    private static string Columns => gg1.RepeatAuto(2);
    [Parameter]
    [EditorRequired]
    public BasicGameCommand? StartPieceChosen { get; set; }
    private async Task PrivateChoseColorAsync(EnumColorChoice color)
    {
        if (StartPieceChosen is null)
        {
            return;
        }
        if (StartPieceChosen.CanExecute(color))
        {
            await StartPieceChosen.ExecuteAsync(color);
            return;
        }
    }
}