namespace Payday.Blazor.Views;
public partial class RollerView
{
    [CascadingParameter]
    public PaydayVMData? VMData { get; set; }
    [CascadingParameter]
    public RollerViewModel? DataContext { get; set; }
    private ICustomCommand RollCommand => DataContext!.RollDiceCommand!;
}