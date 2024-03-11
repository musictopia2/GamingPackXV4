namespace DealCardGame.Blazor;
public partial class StartRentComponent
{
    private RentViewModel? DataContext { get; set; }
    protected override void OnInitialized()
    {
        DataContext = aa1.Resolver!.Resolve<RentViewModel>();
        DataContext.AddAction(StateHasChanged);
    }
    public BasicGameCommand ProcessRentCommand => DataContext!.ProcessRentRequestCommand!;
    public BasicGameCommand CancelCommand => DataContext!.CancelCommand!;

    //if i remove the screens then the basicgameviewmodel would be different because no more screens.

}