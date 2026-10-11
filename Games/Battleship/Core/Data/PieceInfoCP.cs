namespace Battleship.Core.Data;
[UseStateChanged]
public partial class PieceInfoCP
{
    [StateProperty]
    public partial bool DidHit { get; set; }
    [StateProperty]
    public partial string Location { get; set; } = "";
    public int Index { get; set; }
}