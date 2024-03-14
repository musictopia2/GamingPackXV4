namespace DealCardGame.Blazor;
public partial class DisplaySinglePlayerRentComponent
{
    [Parameter]
    [EditorRequired]
    public decimal Owed { get; set; }
    [Parameter]
    [EditorRequired]
    public DealCardGamePlayerItem? PlayerToDisplay { get; set; }
    [Parameter]
    [EditorRequired]
    public EnumRentCategory RentCategory { get; set; }
    private string DisplayName => PlayerToDisplay!.PlayerCategory == EnumPlayerCategory.Self ? "Play Just Say No To Refuse To Pay" : "Play Just Say No To Force The Player To Still Pay";
    private decimal Balance => PlayerToDisplay!.Money >= Owed ? Owed : PlayerToDisplay!.Money;


    private int TimesToRepeat()
    {
        if (RentCategory == EnumRentCategory.SingleDouble)
        {
            return 1;
        }
        if (RentCategory == EnumRentCategory.DoubleDouble)
        {
            return 2;
        }
        return 0;
    }
    private static DealCardGameCardInformation GetDoubleRentCard()
    {
        DealCardGameCardInformation output = new();
        output.Populate(78);
        output.IsUnknown = false;
        return output;
    }

}