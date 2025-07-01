using System.Reflection; //not common enough.   reflection is forced this time.

namespace BasicGameFrameworkLibrary.Blazor.StartupClasses;
public partial class App
{

    public static Assembly? AdditionalAssembly { get; set; }
    private BasicList<Assembly> _additionalAssemblies = [];
    public static string AppStyleName { get; set; } = "";
    private BasicList<string> _files = ["_content/BasicGameFrameworkLibrary/basicgaming.css"];
    protected override void OnInitialized()
    {
        if (AdditionalAssembly is not null)
        {
            _additionalAssemblies =
                [
                    AdditionalAssembly
                ];
        }
        BasicBlazorLibrary.Components.Toasts.BlazoredToasts.Timeout = 3;
    }
}