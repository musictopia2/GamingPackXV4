namespace DealCardGame.Blazor;
public partial class DisplayStolenSetComponent
{
    [Parameter]
    [EditorRequired]
    public DealCardGamePlayerItem? PlayerToDisplay { get; set; }
    [Parameter]
    [EditorRequired]
    public EnumColor Color { get; set; }
    private string DisplayName => PlayerToDisplay!.PlayerCategory == EnumPlayerCategory.Self ? "Your Set Being Stolen" : "Play Just Say No To Receive This Set";
    private BasicList<DealCardGameCardInformation> CardList => PlayerToDisplay!.SetData.GetCards(Color).ToBasicList();
}