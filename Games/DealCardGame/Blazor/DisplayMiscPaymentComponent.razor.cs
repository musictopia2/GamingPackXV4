namespace DealCardGame.Blazor;
public partial class DisplayMiscPaymentComponent
{
    [Parameter]
    [EditorRequired]
    public decimal Owed { get; set; }
    [Parameter]
    [EditorRequired]
    public DealCardGamePlayerItem? PlayerToDisplay { get; set; }
    [Parameter]
    [EditorRequired]
    public DealCardGameCardInformation? ActionCard { get; set; }
    [Parameter]
    [EditorRequired]
    public BasicList<DealCardGamePlayerItem> Players { get; set; } = [];
    private string DisplayName
    {
        get
        {
            if (PlayerToDisplay!.PlayerCategory == EnumPlayerCategory.Self)
            {
                return "Play Just Say No To Refuse To Pay";
            }
            if (ActionCard!.ActionCategory == EnumActionCategory.DebtCollector)
            {
                return "Play Just Say No To Force The Player To Still Pay";
            }
            return "Play Just Say No To Force The Following Players To Pay";
        }
    }
    private bool NeedsPlayerList => ActionCard!.ActionCategory == EnumActionCategory.Birthday;
    //private decimal Balance => PlayerToDisplay!.Money >= Owed ? Owed : PlayerToDisplay!.Money;
    private decimal Balance(DealCardGamePlayerItem player)
    {
        if (player.Money >= Owed)
        {
            return Owed;
        }
        return player.Money;
    }
}