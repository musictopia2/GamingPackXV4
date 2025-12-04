namespace BasicGameFrameworkLibrary.Blazor.GameGraphics.GamePieces;
internal record CircleRecord(string Color, bool NeedsWhiteBorders);
public class CirclePiece : ComponentBase
{
    protected override void OnAfterRender(bool firstRender)
    {
        _previousRecord = new(MainColor, NeedsWhiteBorders);
        base.OnAfterRender(firstRender);
    }
    protected override bool ShouldRender()
    {
        if (MainGraphics!.Animating)
        {
            return true; //because you are doing animations.
        }
        CircleRecord current = new(MainColor, NeedsWhiteBorders);
        return !current.Equals(_previousRecord);
    }
    private CircleRecord? _previousRecord;
    [CascadingParameter]
    public BasePieceGraphics? MainGraphics { get; set; }
    [Parameter]
    public string MainColor { get; set; } = cs1.Transparent;
    [Parameter]
    public bool NeedsWhiteBorders { get; set; }
    protected override void OnInitialized()
    {
        MainGraphics!.OriginalSize = new SizeF(100, 100); //easiest just to use 100 by 100.  of course, its flexible anyways.
        base.OnInitialized();
    }
    protected override void OnParametersSet()
    {
        if (NeedsWhiteBorders)
        {
            MainGraphics!.BorderWidth = 8;
        }
        else
        {
            MainGraphics!.BorderWidth = 4;
        }
        base.OnParametersSet();
    }
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        ISvg svg = MainGraphics!.GetMainSvg(false);
        SvgRenderClass render = new();
        Circle circle;
        if (NeedsWhiteBorders && MainColor != cs1.Transparent)
        {
            circle = new()
            {
                CX = "50",
                CY = "50",
                R = "50"
            };
            circle.PopulateStrokesToStyles(cs1.Black.ToWebColor, (int)MainGraphics!.BorderWidth);
            circle.Fill = cs1.White.ToWebColor;
            svg.Children.Add(circle);
            circle = new()
            {
                CX = "50",
                CY = "50",
                R = "33",
                Fill = MainColor.ToWebColor
            };
            circle.PopulateStrokesToStyles(cs1.Black.ToWebColor, 4);
            svg.Children.Add(circle);
        }
        else
        {
            circle = new()
            {
                CX = "50",
                CY = "50",
                R = "50"
            };
            circle.PopulateStrokesToStyles(cs1.Black.ToWebColor, (int)MainGraphics!.BorderWidth);
            circle.Fill = MainColor.ToWebColor;
            svg.Children.Add(circle);
        }
        render.RenderSvgTree(svg, builder);
        base.BuildRenderTree(builder);
    }
}