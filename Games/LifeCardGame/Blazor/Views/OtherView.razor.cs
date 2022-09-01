namespace LifeCardGame.Blazor.Views;
public partial class OtherView
{
    [CascadingParameter]
    public OtherViewModel? DataContext { get; set; }
    [CascadingParameter]
    public LifeCardGameVMData? GameData { get; set; }
    private ICustomCommand SubmitCommand => DataContext!.ProcessOtherCommand!;
}