namespace Candyland.Blazor;
public class ClickableCandylandSpaceBlazor : ComponentBase
{
    private SizeF _originalSize = new(807, 675);
    [Parameter]
    public EventCallback<int> SpaceClicked { get; set; }
    [Parameter]
    public int SpaceNumber { get; set; }
    [Parameter]
    public CandylandSquareModel? SquareData { get; set; }
    private string GetPathData()
    {
        if (SquareData == null)
        {
            return "";
        }
        return $"M {_originalSize.Width * SquareData.FirstWidth} {_originalSize.Height * SquareData.FirstHeight} L {_originalSize.Width * SquareData.SecondWidth} {_originalSize.Height * SquareData.SecondHeight} L {_originalSize.Width * SquareData.ThirdWidth} {_originalSize.Height * SquareData.ThirdHeight} L {_originalSize.Width * SquareData.FourthWidth} {_originalSize.Height * SquareData.FourthHeight}";
    }
    protected override void BuildRenderTree(RenderTreeBuilder builder) //this is too specialized to use the component i had.
    {
        ISvg svg = new SVG();
        SvgRenderClass render = new();
        svg.EventData.ActionClicked = Clicked;
        Path path = new();
        path.Fill = "aqua";
        path.Fill_Opacity = "0.0";
        path.D = GetPathData();
        svg.Children.Add(path);
        render.RenderSvgTree(svg, 0, builder);
        base.BuildRenderTree(builder);
    }
    private async Task Clicked(object args1, object args2)
    {
        await SpaceClicked.InvokeAsync(SpaceNumber);
    }
}