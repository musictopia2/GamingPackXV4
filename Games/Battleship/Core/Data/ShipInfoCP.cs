namespace Battleship.Core.Data;
public class ShipInfoCP
{
    public string ShipName { get; set; } = "";
    public EnumShipList ShipCategory { get; set; }
    public bool Visible { get; set; }
    public bool IsEnabled { get; set; }
    public Dictionary<int, PieceInfoCP>? PieceList { get; set; }
}