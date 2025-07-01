using BasicGameFrameworkLibrary.Blazor.Bootstrappers;

namespace BasicGameFrameworkLibrary.Blazor.StartupClasses;
public partial class BeginningBlazor
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    [Inject]
    public IJSRuntime? JS { get; set; }
    private EnumGamePackageMode _mode;
    protected override void OnInitialized()
    {
        GlobalStartUp.JsRuntime = JS;
        if (GlobalStartUp.StartBootStrap == null)
        {
            throw new CustomBasicException("There was nothing to start up the bootstrap");
        }
        GlobalStartUp.StartBootStrap.Invoke();
        _mode = GlobalVariables.GetGamePackageMode;
        base.OnInitialized();
    }
}