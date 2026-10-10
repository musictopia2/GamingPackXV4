using BasicGameFrameworkLibrary.Blazor.Bootstrappers;

namespace BasicGameFrameworkLibrary.Blazor.StartupClasses;
public partial class BeginningBlazor
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }


    [Inject]
    public IJSRuntime? JsRuntime { get; set; }

    private EnumGamePackageMode? _mode;

    protected override async Task OnInitializedAsync()
    {
        GlobalStartUp.JsRuntime = JsRuntime;
        if (GlobalStartUp.StartBootStrap == null)
        {
            throw new CustomBasicException("There was nothing to start up the bootstrap");
        }
        await GlobalStartUp.StartBootStrap.Invoke();
        //_mode = EnumGamePackageMode.Debug;
        _mode = GlobalVariables.GetGamePackageMode;
    }
}