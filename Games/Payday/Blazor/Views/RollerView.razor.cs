namespace Payday.Blazor.Views;
public partial class RollerView
{
    [CascadingParameter]
    public PaydayVMData? VMData { get; set; }
    private ICustomCommand RollCommand => DataContext!.RollDiceCommand!;
}