namespace DealCardGame.Blazor;
public partial class StartRentComponent
{
    private RentViewModel? DataContext { get; set; }
    protected override void OnInitialized()
    {
        DataContext = aa1.Resolver!.Resolve<RentViewModel>();
    }

    public BasicGameCommand ProcessRentCommand => DataContext!.ProcessRentRequestCommand!;
    public BasicGameCommand CancelCommand => DataContext!.CancelCommand!;

}