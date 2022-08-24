namespace BasicGameFrameworkLibrary.Blazor.BasicControls.GameBoards;
public partial class CheckersChessBoardBlazor<E, S>
    where E : IFastEnumColorSimple
    where S : CheckersChessSpace<E>, new()
{
    [Parameter]
    public string TargetHeight { get; set; } = "";
    [Parameter]
    public string TargetWidth { get; set; } = "";
    [Parameter]
    public float X { get; set; }
    [Parameter]
    public float Y { get; set; }
    public SizeF GetSize => CheckersChessBaseBoard<E, S>.OriginalSize;
    [Parameter]
    public RenderFragment<CheckerChessPieceCP<E>>? MainPiece { get; set; }
    [Parameter]
    public RenderFragment<S>? HighlightPiece { get; set; }
    [Parameter]
    public RenderFragment? AnimatePiece { get; set; }
    [Parameter]
    public bool CanRenderSpace { get; set; } = true;
    [Parameter]
    public CheckersChessBaseBoard<E, S>? GameBoard { get; set; }
    private static async Task MakeMoveAsync(int index)
    {
        if (CheckersChessDelegates.CanMove == null || CheckersChessDelegates.MakeMoveAsync == null)
        {
            return;
        }
        if (CheckersChessDelegates.CanMove.Invoke() == false)
        {
            return;
        }
        await CheckersChessDelegates.MakeMoveAsync.Invoke(index);
    }
}