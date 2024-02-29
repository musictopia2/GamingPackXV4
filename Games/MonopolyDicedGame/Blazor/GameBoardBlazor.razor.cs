namespace MonopolyDicedGame.Blazor;
public partial class GameBoardBlazor : ComponentBase
{
    [Parameter]
    public string TargetHeight { get; set; } = "";
    [Parameter]
    public string ImageHeight { get; set; } = ""; //i am forced to do this too unfortunately.
    [Parameter]
    public BasicList<EnumMiscType> MiscList { get; set; } = [];
    [Parameter]
    public int NumberOfCops { get; set; }
    [Parameter]
    public EnumMiscType HouseValue { get; set; }
    [Parameter]
    public bool HasHotel { get; set; }
    [Parameter]
    public int HousesOwned { get; set; }
    //next version may do some rethinking.

    private SizeF _size = new(400, 400);
    private static BasicDiceModel GetWaterDice()
    {
        BasicDiceModel output = new();
        output.Populate(9);
        return output;
    }
    private static BasicDiceModel GetElectricDice()
    {
        BasicDiceModel output = new();
        output.Populate(10);
        return output;
    }
    private static string GetUtilityColor => cc1.Black.ToWebColor();
    private static string GetUtilityBorder => cc1.White.ToWebColor();
}