namespace SkuckCardGame.Blazor.Views;
public partial class SkuckSuitView
{
    private ICustomCommand TrumpCommand => DataContext!.TrumpCommand!;
}