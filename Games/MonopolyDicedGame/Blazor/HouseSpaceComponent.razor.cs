namespace MonopolyDicedGame.Blazor;
public partial class HouseSpaceComponent
{
    [Parameter]
    public string ImageHeight { get; set; } = "";
    [Parameter]
    public int HousesOwned { get; set; }
    [Parameter]
    public bool HasHotel { get; set; }
    //if you have hotel will use that instead.

}