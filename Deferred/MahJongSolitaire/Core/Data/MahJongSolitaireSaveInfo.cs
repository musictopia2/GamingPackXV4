namespace MahJongSolitaire.Core.Data;
[SingletonGame]
public class MahJongSolitaireSaveInfo : IMappable
{
    public int FirstSelected { get; set; }
    public int TilesGone { get; set; }
    public BasicList<BoardInfo> BoardList { get; set; } = new();
    public BasicList<BoardInfo> PreviousList { get; set; } = new(); //this is needed to support undo move.
}