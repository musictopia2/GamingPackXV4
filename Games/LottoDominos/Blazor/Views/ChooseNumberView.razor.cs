namespace LottoDominos.Blazor.Views;
public partial class ChooseNumberView
{
    [CascadingParameter]
    public ChooseNumberViewModel? DataContext { get; set; }
    private ICustomCommand ChooseCommand => DataContext!.ChooseNumberCommand!;
}