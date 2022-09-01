namespace SkuckCardGame.Blazor.Views;
public partial class SkuckChoosePlayView
{
    private ICustomCommand PlayCommand => DataContext!.FirstPlayCommand!;
}