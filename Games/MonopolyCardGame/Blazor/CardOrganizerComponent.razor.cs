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
    private BasicList<MonopolyCardGameCardInformation> _calculatorStart = [];
    private BasicList<MonopolyCardGameCardInformation> _allOwned = [];
    private ICustomCommand PutBackCommand => DataContext!.PutBackCommand!;
    private ICustomCommand ManuelCommand => DataContext!.ManuallyPlaySetsCommand!;
    private MonopolyCardGameGameContainer? _container;
    //private BasicList<CalculatorModel> _calculations = [];
    protected override void OnInitialized()
    {
        Model!.TempHand1.AutoSelect = EnumHandAutoType.SelectAsMany;
        _container = aa1.Resolver!.Resolve<MonopolyCardGameGameContainer>();
        Model.Calculator1.StateHasChanged = StateHasChanged;
    }
    private void CreateNewCalculator()
    {
        _allOwned = GetYourCards();
        _calculatorStart = Model!.Calculator1.StartNewEntry(_container!.DeckList);
    }
    private BasicList<CalculatorModel> GetCalculations => Model!.Calculator1.GetTotalCalculations;
    private void ClearCalculator()
    {
        Model!.Calculator1.ClearCalculator();
    }
    private BasicList<MonopolyCardGameCardInformation> GetYourCards()
    {
        //maybe don't need to clone this time though.
        var output = Player!.MainHandList.ToBasicList();
        output.AddRange(Model!.TempSets1.ListAllObjects());
        return output;
    }
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