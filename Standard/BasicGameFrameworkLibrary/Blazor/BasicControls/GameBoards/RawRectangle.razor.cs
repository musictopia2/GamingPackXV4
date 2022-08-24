namespace BasicGameFrameworkLibrary.Blazor.BasicControls.GameBoards;
public partial class RawRectangle
{
    [Parameter]
    public RectangleF Rectangle { get; set; }
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}