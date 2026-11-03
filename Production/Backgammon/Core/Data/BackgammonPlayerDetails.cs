namespace Backgammon.Core.Data;
public class BackgammonPlayerDetails
{
    public BasicList<int>? PiecesOnBoard { get; set; }
    public int PiecesAtHome { get; set; }
    public int PiecesAtStart { get; set; }
}