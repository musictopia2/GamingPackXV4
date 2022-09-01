namespace Fluxx.Blazor.Views;
public partial class ActionFirstCardRandomView : SimpleActionView
{
    [CascadingParameter]
    public ActionFirstCardRandomViewModel? DataContext { get; set; }
    private ICustomCommand ChooseCommand => DataContext!.ChooseCardCommand!;
}