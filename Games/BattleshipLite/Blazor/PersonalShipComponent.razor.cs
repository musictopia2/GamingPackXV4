namespace BattleshipLite.Blazor;
public partial class PersonalShipComponent
{
    [CascadingParameter]
    private BattleshipLiteMainViewModel? DataContext { get; set; }
    [CascadingParameter]
    private string TargetHeight { get; set; } = "";
    private string PieceColor(ShipInfo ship)
    {
        var opponent = DataContext!.OpponentShip(ship);
        if (opponent.ShipStatus == EnumShipStatus.Hit)
        {
            return cs.Red.ToWebColor();
        }
        return cs.Gray.ToWebColor();
    }
}