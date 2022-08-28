using System.Reflection; //okay to keep reflection here.
namespace LifeBoardGame.Blazor;
public partial class SpinnerGraphicsBlazor
{
    [Parameter]
    public string TargetHeight { get; set; } = "";
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    private Assembly GetAssembly => Assembly.GetAssembly(GetType())!;
}