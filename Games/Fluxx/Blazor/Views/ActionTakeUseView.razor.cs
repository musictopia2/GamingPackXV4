namespace Fluxx.Blazor.Views;
public partial class ActionTakeUseView : SimpleActionView<ActionTakeUseViewModel>
{
    private ICustomCommand ChooseCommand => DataContext!.ChooseCardCommand!;
}