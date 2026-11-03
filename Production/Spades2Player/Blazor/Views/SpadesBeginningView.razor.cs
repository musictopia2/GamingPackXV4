namespace Spades2Player.Blazor.Views;
public partial class SpadesBeginningView
{
    private ICustomCommand TakeCommand => DataContext!.TakeCardCommand!;
}