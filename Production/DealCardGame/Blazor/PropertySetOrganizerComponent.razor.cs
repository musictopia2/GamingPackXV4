namespace DealCardGame.Blazor;
public partial class PropertySetOrganizerComponent
{
    private YourOrganizerViewModel? DataContext { get; set; }
    [CascadingParameter]
    public int ImageHeight { get; set; }
    protected override void OnInitialized()
    {
        DataContext = Resolver!.Resolve<YourOrganizerViewModel>();



        //DataContext.AddAction(StateHasChanged);
        DataContext.VMData.YourCompleteSets.Init(); //i think.
        base.OnInitialized();
    }
    private BasicGameCommand ResetCommand => DataContext!.ResetCommand!;
    private BasicGameCommand CancelCommand => DataContext!.CancelCommand!;
    private BasicGameCommand FinishCommand => DataContext!.FinishCommand!;
    private BasicGameCommand PutToTemporaryHandCommand => DataContext!.PutToTemporaryHandCommand!;
    private PropertySetHand GetHand(int index) => DataContext!.VMData.YourCompleteSets.SetList[index];

    private static string StackHeight => $"{65.HeightPixels}px";
    private static string HandHeight => $"{50.HeightPixels}px";


}