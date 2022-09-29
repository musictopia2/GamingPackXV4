using System.Reflection; //not common enough.
namespace Xactika.Blazor;
public class PieceBlazor : ComponentBase
{
    [CascadingParameter]
    public BasePieceGraphics? MainGraphics { get; set; }
    [Parameter]
    public int HowMany { get; set; }
    [Parameter]
    public EnumShapes ShapeUsed { get; set; }
    protected override void OnInitialized()
    {
        MainGraphics!.OriginalSize = new SizeF(60, 138);
        MainGraphics.NeedsHighlighting = true;
        base.OnInitialized();
    }
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (ShapeUsed == EnumShapes.None)
        {
            return;
        }
        ISvg svg = MainGraphics!.GetMainSvg(false);
        SvgRenderClass render = new();
        Assembly assembly = Assembly.GetAssembly(GetType())!;
        var thisHeight = 40;
        var thisSize = new SizeF(thisHeight, thisHeight);
        var pointList = ImageHelpers.GetPoints(ShapeUsed, HowMany, MainGraphics.Location, true, thisHeight); // can always be adjusted.   test on desktop first anyways.
        foreach (var thisPoint in pointList)
        {
            var thisRect = new RectangleF(thisPoint, thisSize);
            if (ShapeUsed == EnumShapes.Balls)
            {
                ImageHelpers.DrawBall(svg, thisRect);
            }
            else if (ShapeUsed == EnumShapes.Cubes)
            {
                ImageHelpers.DrawCube(svg, assembly, thisRect);
            }
            else if (ShapeUsed == EnumShapes.Cones)
            {
                ImageHelpers.DrawCone(svg, assembly, thisRect);
            }
            else if (ShapeUsed == EnumShapes.Stars)
            {
                //svg.DrawStar(thisRect, )
                ImageHelpers.DrawStar(svg, thisRect);
            }
        }
        render.RenderSvgTree(svg, builder);
        base.BuildRenderTree(builder);
    }
}