namespace Backgammon.Blazor;
public partial class BackgammonCompleteBoard
{
    [CascadingParameter]
    public GameBoardGraphicsCP? GraphicsData { get; set; } //decided to cascade it because something else needs it as well.
    [Parameter]
    public string TargetHeight { get; set; } = "";
    private string GetColor() => GetColor(GraphicsData!.GameContainer.SingleInfo!);
    private static string GetColor(BackgammonPlayerItem player)
    {
        return player.Color.Color;
    }
    private static EnumCheckerPieceCategory GetCategory(int index)
    {
        if (index == 25 || index == 26)
        {
            return EnumCheckerPieceCategory.FlatPiece;
        }
        return EnumCheckerPieceCategory.OnlyPiece;
    }
    protected override bool ShouldRender()
    {
        return GraphicsData!.GameContainer.SingleInfo!.Color != EnumColorChoice.None;
    }
    private static float GetSpaceWidth => GameBoardGraphicsCP.SpaceSize.Width;
    private static SizeF OriginalSize => GameBoardGraphicsCP.OriginalSize;
}