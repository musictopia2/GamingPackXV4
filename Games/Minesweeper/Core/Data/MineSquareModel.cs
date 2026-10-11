namespace Minesweeper.Core.Data;
public class MineSquareModel(int pColumn, int pRow)
{
    public bool IsMine { get; set; }
    public int NeighborMines { get; set; }
    public bool Flagged { get; set; }
    public bool Pressed { get; set; }
    public bool IsFlipped { get; set; }
    public int Column { get; set; } = pColumn;
    public int Row { get; set; } = pRow;
}