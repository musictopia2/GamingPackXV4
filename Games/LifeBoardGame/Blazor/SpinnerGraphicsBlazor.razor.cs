namespace LifeBoardGame.Blazor;
public partial class SpinnerGraphicsBlazor
{
    [Parameter]
    public string TargetHeight { get; set; } = "";
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}