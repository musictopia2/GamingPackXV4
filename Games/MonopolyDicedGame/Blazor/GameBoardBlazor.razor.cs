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
    [Parameter]
    public BasicList<OwnedModel> OwnList { get; set; } = [];

    //[Parameter]
    //public int TrainsOwned { get; set; }
    //[Parameter]
    //public int UtilitiesOwned { get; set; }
    [Parameter]
    public bool IsEnabled { get; set; }
    [Parameter]
    public EventCallback<EnumUtilityType> UtilityClicked { get; set; }
    [Parameter]
    public EventCallback TrainClicked { get; set; }
    [Parameter]
    public EventCallback<int> PropertyClicked { get; set; }

    //next version may do some rethinking.

    private SizeF _size = new(400, 400);
    private void PossibleUtilityClick(EnumUtilityType utility)
    {
        if (IsEnabled == false)
        {
            return;
        }
        UtilityClicked.InvokeAsync(utility);
    }
    private void PossibleTrainClick()
    {
        if (IsEnabled == false)
        {
            return;
        }
        TrainClicked.InvokeAsync();
    }
    private void PossiblePropertyClick(int group)
    {
        if (IsEnabled == false)
        {
            return;
        }
        PropertyClicked.InvokeAsync(group);
    }


}