namespace Battleship.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class BattleshipVMData : IViewModelData
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    public EnumShipList ShipSelected { get; set; }
    public bool ShipDirectionsVisible { get; set; }

}