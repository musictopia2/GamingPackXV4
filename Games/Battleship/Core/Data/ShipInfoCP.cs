namespace Battleship.Core.Data;
[UseStateChanged]
public partial class ShipInfoCP
{
    [StateProperty]
    public partial string ShipName { get; set; } = "";
    [StateProperty]
    public partial EnumShipList ShipCategory { get; set; }
    [StateProperty]
    public partial bool Visible { get; set; }
    public bool IsEnabled { get; set; }
    public Dictionary<int, PieceInfoCP>? PieceList { get; set; }
}