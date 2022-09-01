namespace Fluxx.Blazor.Views;
public partial class ActionDoAgainView : SimpleActionView
{
    [CascadingParameter]
    public ActionDoAgainViewModel? DataContext { get; set; }
    private ICustomCommand SelectCommand => DataContext!.SelectCardCommand!;
    private ICustomCommand ViewCommand => DataContext!.ViewCardCommand!;
}