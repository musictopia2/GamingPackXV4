namespace BasicGameFrameworkLibrary.Blazor.StartupClasses;
public partial class MainPage
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

}