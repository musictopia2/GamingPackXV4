namespace Fluxx.Blazor.Views;
public partial class ActionTakeUseView : SimpleActionView
{
    [CascadingParameter]
    public ActionTakeUseViewModel? DataContext { get; set; }
    private ICustomCommand ChooseCommand => DataContext!.ChooseCardCommand!;
}