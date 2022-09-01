namespace Fluxx.Blazor.Views;
public partial class ActionDrawUseView : SimpleActionView
{
    [CascadingParameter]
    public ActionDrawUseViewModel? DataContext { get; set; }
    private ICustomCommand DrawUseCommand => DataContext!.DrawUseCommand!;
}