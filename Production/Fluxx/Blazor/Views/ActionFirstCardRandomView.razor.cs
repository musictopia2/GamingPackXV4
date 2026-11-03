namespace Fluxx.Blazor.Views;
public partial class ActionFirstCardRandomView : SimpleActionView<ActionFirstCardRandomViewModel>
{
    private ICustomCommand ChooseCommand => DataContext!.ChooseCardCommand!;
}