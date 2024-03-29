namespace BasicGameFrameworkLibrary.Blazor.GameGraphics.Base;
public partial class BorderedCommandSpace
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    [Parameter]
    public int BorderWidth { get; set; }
    [Parameter]
    public string BorderColor { get; set; } = "";
    [Parameter]
    public string FillColor { get; set; } = cs1.Transparent;
    private string ShapeStyle()
    {
        string output = $"stroke: {BorderColor.ToWebColor()}; stroke-width: {BorderWidth}; stroke-miterlimit:4;";
        return output;
    }
}