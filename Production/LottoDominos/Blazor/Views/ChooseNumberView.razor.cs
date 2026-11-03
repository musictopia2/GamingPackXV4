namespace LottoDominos.Blazor.Views;
public partial class ChooseNumberView
{
    private ICustomCommand ChooseCommand => DataContext!.ChooseNumberCommand!;
}