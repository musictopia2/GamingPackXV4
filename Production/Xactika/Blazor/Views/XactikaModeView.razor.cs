namespace Xactika.Blazor.Views;
public partial class XactikaModeView
{
    private ICustomCommand SubmitCommand => DataContext!.ModeCommand!;
}