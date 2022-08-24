namespace BasicGameFrameworkLibrary.Blazor.Views;
public partial class RestoreView
{
    [CascadingParameter]
    public RestoreViewModel? DataContext { get; set; }
}