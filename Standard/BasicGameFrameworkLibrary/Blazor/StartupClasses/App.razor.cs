namespace BasicGameFrameworkLibrary.Blazor.StartupClasses;
public partial class App
{
    public static string AppStyleName { get; set; } = "";
    private BasicList<string> _files = ["_content/BasicGameFrameworkLibrary/basicgaming.css"];
    protected override void OnInitialized()
    {
        BasicBlazorLibrary.Components.Toasts.BlazoredToasts.Timeout = 3;
    }
}