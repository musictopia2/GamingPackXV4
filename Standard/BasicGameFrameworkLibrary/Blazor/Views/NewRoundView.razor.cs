namespace BasicGameFrameworkLibrary.Blazor.Views;
public partial class NewRoundView
{
    [CascadingParameter]
    public NewRoundViewModel? DataContext { get; set; }
}