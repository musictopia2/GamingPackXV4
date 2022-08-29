namespace GoFish.Blazor.Views;
public partial class AskView
{
    [CascadingParameter]
    public GoFishVMData? Model { get; set; }
    [CascadingParameter]
    public AskViewModel? DataContext { get; set; }
    private ICustomCommand AskCommand => DataContext!.AskCommand!;
}