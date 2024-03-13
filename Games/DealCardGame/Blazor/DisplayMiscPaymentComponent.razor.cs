namespace DealCardGame.Blazor;
public partial class DisplayMiscPaymentComponent
{
    [Parameter]
    [EditorRequired]
    public decimal Owed { get; set; }
    [Parameter]
    [EditorRequired]
    public DealCardGamePlayerItem? PlayerToDisplay { get; set; }
    private string DisplayName => PlayerToDisplay!.PlayerCategory == EnumPlayerCategory.Self ? "Play Just Say No To Refuse To Pay" : "Play Just Say No To Force The Player To Still Pay";
    private decimal Balance => PlayerToDisplay!.Money >= Owed ? Owed : PlayerToDisplay!.Money;
}