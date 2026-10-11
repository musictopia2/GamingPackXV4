namespace BattleshipLite.Blazor;
public partial class PersonalShipPiece
{
    [Parameter]
    [EditorRequired]
    public ShipInfo YourShip { get; set; }

    [Parameter]
    [EditorRequired]
    public int Y { get; set; }

    [Parameter]
    [EditorRequired]
    public ShipInfo OpponentShip { get; set; }


    private string PieceColor(ShipInfo ship)
    {
        if (OpponentShip.ShipStatus == EnumShipStatus.Hit)
        {
            return cs1.Red.ToWebColor;
        }
        return cs1.Gray.ToWebColor;
    }

}