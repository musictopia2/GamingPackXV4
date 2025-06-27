namespace BasicGameFrameworkLibrary.Blazor.StartupClasses;
public partial class GameCssStartup
{
    private BasicList<string> _files = ["_content/BasicGameFrameworkLibrary/basicgaming.css"];
    [Parameter]
    public string AppStyleName { get; set; } = "";
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    protected override void OnInitialized()
    {
        BasicBlazorLibrary.Components.Toasts.BlazoredToasts.Timeout = 3;
        base.OnInitialized();
    }
}