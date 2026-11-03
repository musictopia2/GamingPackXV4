namespace Fluxx.Blazor.Views;
public partial class ActionDoAgainView : SimpleActionView<ActionDoAgainViewModel>
{
    private ICustomCommand SelectCommand => DataContext!.SelectCardCommand!;
    private ICustomCommand ViewCommand => DataContext!.ViewCardCommand!;
}