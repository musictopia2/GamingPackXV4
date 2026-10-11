namespace Fluxx.Blazor.Views;
public partial class ActionDrawUseView : SimpleActionView<ActionDrawUseViewModel>
{
    private ICustomCommand DrawUseCommand => DataContext!.DrawUseCommand!;
}