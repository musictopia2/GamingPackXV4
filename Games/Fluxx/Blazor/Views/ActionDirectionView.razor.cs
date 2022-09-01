namespace Fluxx.Blazor.Views;
public partial class ActionDirectionView : SimpleActionView
{
    [CascadingParameter]
    public ActionDirectionViewModel? DataContext { get; set; }
    private ICustomCommand DirectionCommand => DataContext!.DirectionCommand!;
}