namespace GoFish.Blazor.Views;
public partial class AskView
{
    [CascadingParameter]
    public GoFishVMData? Model { get; set; }
    private ICustomCommand AskCommand => DataContext!.AskCommand!;
}