namespace Battleship.Blazor;
public partial class ShipPieceBlazor
{
    [EditorRequired]
    [Parameter]
    public int X { get; set; }
    [EditorRequired]
    [Parameter]
    public PieceInfoCP Piece { get; set; }
    private string PieceColor => Piece.DidHit ? cs1.Red.ToWebColor : cs1.Gray.ToWebColor;
}