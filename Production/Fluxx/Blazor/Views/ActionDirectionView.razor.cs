namespace Fluxx.Blazor.Views;
public partial class ActionDirectionView : SimpleActionView<ActionDirectionViewModel>
{
    private ICustomCommand DirectionCommand => DataContext!.DirectionCommand!;
}