namespace BasicGameFrameworkLibrary.Blazor.BasicControls.GameBoards;
public class SimpleTriangle : ComponentBase
{
    public enum EnumTriangleInfo
    {
        BottomLeft,
        BottomCenter,
        BottomRight,
        TopLeft,
        TopCenter,
        TopRight
    }
    [Parameter]
    public EnumTriangleInfo TriangleInfo { get; set; }
    [Parameter]
    public RectangleF Bounds { get; set; }
    [Parameter]
    public string BorderColor { get; set; } = cs1.Transparent;
    [Parameter]
    public string FillColor { get; set; } = cs1.Transparent;
    [Parameter]
    public int BorderWidth { get; set; } = 3;
    private float GetCenterX => (Bounds.X + Bounds.Right) / 2;
    private BasicList<PointF> GetTrianglePoints()
    {
        PointF topLeft;
        PointF topCenter;
        PointF topRight;
        PointF bottomLeft;
        PointF bottomCenter;
        PointF bottomRight;
        topLeft = new PointF(Bounds.X, Bounds.Y);
        topCenter = new PointF(GetCenterX, Bounds.Y);
        topRight = new PointF(Bounds.Right, Bounds.Y);
        bottomLeft = new PointF(Bounds.X, Bounds.Bottom);
        bottomCenter = new PointF(GetCenterX, Bounds.Bottom);
        bottomRight = new PointF(Bounds.Right, Bounds.Bottom);
        return TriangleInfo switch
        {
            EnumTriangleInfo.BottomLeft => new BasicList<PointF>() { bottomLeft, topLeft, bottomRight },
            EnumTriangleInfo.BottomCenter => new BasicList<PointF>() { bottomLeft, topCenter, bottomRight },
            EnumTriangleInfo.BottomRight => new BasicList<PointF>() { bottomLeft, topRight, bottomRight },
            EnumTriangleInfo.TopLeft => new BasicList<PointF>() { topLeft, bottomLeft, topRight },
            EnumTriangleInfo.TopCenter => new BasicList<PointF>() { topLeft, bottomCenter, topRight },
            EnumTriangleInfo.TopRight => new BasicList<PointF>() { topLeft, bottomRight, topRight },
            _ => new BasicList<PointF>() { },
        };
    }
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (BorderColor == cs1.Transparent && FillColor == cs1.Transparent)
        {
            return;
        }
        SvgRenderClass render = new();
        ISvg svg = new SVG();
        BasicList<PointF> points = GetTrianglePoints();
        Polygon poly = points.CreatePolygon();
        poly.Fill = FillColor.ToWebColor(); //i think.
        if (BorderColor != cs1.Transparent)
        {
            poly.PopulateStrokesToStyles(BorderColor.ToWebColor(), BorderWidth);
        }
        svg.Children.Add(poly);
        render.RenderSvgTree(svg, builder);
        base.BuildRenderTree(builder);
    }
    protected override bool ShouldRender()
    {
        return false;
    }
}