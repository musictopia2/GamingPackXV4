namespace ChineseCheckers.Blazor;
public record PlayerPieceRecord(ChineseCheckersPlayerItem Player, RectangleF Bounds);
public partial class GameBoardBlazor
{
    [Parameter]
    public GameBoardGraphicsCP? Graphics { get; set; }
    [Parameter]
    public ChineseCheckersGameContainer? Container { get; set; }
    [Parameter]
    public string TargetHeight { get; set; } = "";
    private static string Color(ChineseCheckersPlayerItem player)
    {
        string output = player.Color.Color;
        return output;
    }
    private async Task SpaceClickedAsync(int index)
    {
        if (Container!.MakeMoveAsync == null)
        {
            return;
        }
        await Container.MakeMoveAsync(index);
    }
    protected override bool ShouldRender()
    {
        return Container!.SingleInfo!.Color != EnumColorChoice.None;
    }
}
