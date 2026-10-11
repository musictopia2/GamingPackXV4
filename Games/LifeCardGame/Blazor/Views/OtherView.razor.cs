namespace LifeCardGame.Blazor.Views;
public partial class OtherView
{
    [CascadingParameter]
    public LifeCardGameVMData? GameData { get; set; }
    private ICustomCommand SubmitCommand => DataContext!.ProcessOtherCommand!;
}