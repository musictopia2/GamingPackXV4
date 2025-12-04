namespace BasicGameFrameworkLibrary.Blazor.BasicControls.GameBoards;
public partial class BorderedSpace
{
    public enum EnumShapeCategory
    {
        Rectangle,
        Oval //decided for oval instead of circle to be more flexible.  if you want circle, then make the width and height the same.
    }
    [Parameter]
    public SizeF SpaceSize { get; set; }
    [Parameter]
    public EnumShapeCategory ShapeCategory { get; set; }
    [Parameter]
    public EventCallback SpaceClicked { get; set; }
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    [Parameter]
    public int BorderWidth { get; set; }
    [Parameter]
    public string BorderColor { get; set; } = ""; //if no bordercolor, then don't even use this.
    [Parameter]
    public string FillColor { get; set; } = cs1.Transparent;
    [Parameter]
    public bool Fixed { get; set; }
    private async Task ProcessClickAsync()
    {
        if (SpaceClicked.HasDelegate == false)
        {
            return;
        }
        await SpaceClicked.InvokeAsync(null);
    }
    protected override bool ShouldRender()
    {
        return !Fixed;
    }
    [Parameter]
    public PointF SpaceLocation { get; set; }
    private string ViewBox()
    {
        var value = BorderWidth / 2 * -1;
        return $"{value} {value} {SpaceSize.Width + BorderWidth} {SpaceSize.Height + BorderWidth}";
    }
    private float RadiusX => SpaceSize.Width / 2;
    private float RadiusY => SpaceSize.Height / 2;
    private string ShapeStyle()
    {
        string output = $"stroke: {BorderColor.ToWebColor}; stroke-width: {BorderWidth}; stroke-miterlimit:4;";
        return output;
    }
}