namespace PassOutDiceGame.Blazor;
public partial class GameBoardBlazor
{
    [Parameter]
    public GameBoardGraphicsCP? GraphicsData { get; set; }
    [Parameter]
    public string TargetHeight { get; set; } = "";
    private async Task SpaceClicked(int space)
    {
        await GraphicsData!.GameContainer.ProcessCustomCommandAsync(GraphicsData.GameContainer.MakeMoveAsync!, space);
    }
}