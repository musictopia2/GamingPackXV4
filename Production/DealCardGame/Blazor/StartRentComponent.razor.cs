namespace DealCardGame.Blazor;
public partial class StartRentComponent
{
    private RentViewModel? DataContext { get; set; }
    protected override void OnInitialized()
    {
        DataContext = Resolver!.Resolve<RentViewModel>();
        //DataContext.AddAction(StateHasChanged);
    }
    public BasicGameCommand ProcessRentCommand => DataContext!.ProcessRentRequestCommand!;
    public BasicGameCommand CancelCommand => DataContext!.CancelCommand!;
}