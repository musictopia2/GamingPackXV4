namespace DealCardGame.Blazor;
public partial class PropertySetOrganizerComponent
{
    private YourOrganizerViewModel? DataContext { get; set; }
    protected override void OnInitialized()
    {
        DataContext = aa1.Resolver!.Resolve<YourOrganizerViewModel>();
        DataContext.AddAction(StateHasChanged);
        DataContext.VMData.YourCompleteSets.Init(); //i think.
        base.OnInitialized();
    }
    private BasicGameCommand ResetCommand => DataContext!.ResetCommand!;
    private BasicGameCommand CancelCommand => DataContext!.CancelCommand!;
    private BasicGameCommand FinishCommand => DataContext!.FinishCommand!;
    private BasicGameCommand PutToTemporaryHandCommand => DataContext!.PutToTemporaryHandCommand!;
    private PropertySetHand GetHand(int index) => DataContext!.VMData.YourCompleteSets.SetList[index];
}