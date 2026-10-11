namespace Battleship.Core.Data;
[SingletonGame]
[UseLabelGrid]
[UseStateChanged]
public partial class BattleshipVMData : IViewModelData
{
    [LabelColumn]
    public partial string NormalTurn { get; set; } = "";
    [LabelColumn]
    public partial string Status { get; set; } = "";
    [StateProperty]
    public partial EnumShipList ShipSelected { get; set; }
    public bool ShipDirectionsVisible { get; set; }
}