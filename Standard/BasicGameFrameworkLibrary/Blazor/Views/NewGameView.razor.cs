namespace BasicGameFrameworkLibrary.Blazor.Views;
public partial class NewGameView
{
    [CascadingParameter]
    public NewGameViewModel? DataContext { get; set; }
}