namespace LifeBoardGame.Blazor;
public partial class SpinnerGraphicsBlazor
{
    [Parameter]
    public double TargetHeight { get; set; }
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}