namespace Uno.Blazor.Views;
public partial class SayUnoView
{
    private ICustomCommand UnoCommand => DataContext!.SayUnoCommand!;
}