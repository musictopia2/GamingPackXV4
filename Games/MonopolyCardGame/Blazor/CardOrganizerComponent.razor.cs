namespace MonopolyCardGame.Blazor;
public partial class CardOrganizerComponent
{
    [Parameter]
    public EnumManuelStatus Status { get; set; }
    [Parameter]
    [EditorRequired]
    public MonopolyCardGamePlayerItem? Player { get; set; }
    [Parameter]
    public EventCallback OnOrganizedCards { get; set; }
    [CascadingParameter]
    public MonopolyCardGameVMData? Model { get; set; }

    //private MonopolyCardGameVMData? _vmData;
    private ICustomCommand PutBackCommand => DataContext!.PutBackCommand!;
    private ICustomCommand ManuelCommand => DataContext!.ManuallyPlaySetsCommand!;
    protected override void OnInitialized()
    {
        Model!.TempHand1.AutoSelect = EnumHandAutoType.SelectAsMany;
    }
    //private void PopulateManuelCards()
    //{
    //    Player!.PopulateManuelCards(_vmData!, false);
    //    DataContext!.MainGame.SortTempHand();
    //}
    private string GetInstructions()
    {
        if (Status == EnumManuelStatus.OrganizingCards)
        {
            return "Organize your cards to help you go out";
        }
        if (Status == EnumManuelStatus.Final)
        {
            return "Put down your monopolies including houses, hotels, and tokens since you went you.  You must use up all tokens and put down all obvious monopolies";
        }
        if (Status == EnumManuelStatus.OthersLayingDown)
        {
            return "Put down any monopolies you may have.   You must put down all obvious monopolies.  If you have at least one monopoly, you must use up all tokens";
        }
        return "Unknown";
    }
}