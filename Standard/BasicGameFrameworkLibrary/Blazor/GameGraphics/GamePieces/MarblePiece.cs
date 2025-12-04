namespace BasicGameFrameworkLibrary.Blazor.GameGraphics.GamePieces;
public class MarblePiece : ComponentBase
{
    private ColorRecord? _previousRecord;
    private ColorRecord GetRecord => new(MainColor, MainGraphics!.IsSelected, MainGraphics.CustomCanDo.Invoke());
    protected override void OnAfterRender(bool firstRender)
    {
        _previousRecord = GetRecord;
        base.OnAfterRender(firstRender);
    }
    protected override bool ShouldRender()
    {
        if (_previousRecord is null)
        {
            return true;
        }
        if (MainGraphics!.Animating || MainGraphics.ForceRender)
        {
            return true; //because you are doing animations.
        }
        return _previousRecord!.Equals(GetRecord) == false;
    }
    [CascadingParameter]
    public BasePieceGraphics? MainGraphics { get; set; }
    [Parameter]
    public string MainColor { get; set; } = cs1.Transparent;
    protected override void OnInitialized()
    {
        MainGraphics!.OriginalSize = new SizeF(150, 150); //decided to use 150 by 150 this time.
        MainGraphics.BorderWidth = 1;
        MainGraphics.HighlightTransparent = true;
        base.OnInitialized();
    }
    private void BuildRadicalDefs(ISvg svg)
    {
        Defs defs = new();
        RadialGradient radial = new()
        {
            ID = $"grad{MainColor}",
            CX = "50%",
            CY = "50%",
            R = "50%",
            FX = "50%",
            FY = "50%"
        };
        defs.Children.Add(radial);
        svg.Children.Add(defs);
        Stop stop = new();
        stop.Offset = "0%";
        stop.Stop_Color = "rgb(255,255,255)";
        stop.Stop_Opacity = "1";
        radial.Children.Add(stop);
        stop = new();
        stop.Offset = "100%";
        stop.Stop_Color = MainColor.ToWebColor;
        stop.Stop_Opacity = "1";
        radial.Children.Add(stop);
    }
    private void DrawRegularMarblePiece(ISvg svg)
    {
        BuildRadicalDefs(svg);
        Ellipse ellipse = new();
        ellipse.CX = "75";
        ellipse.CY = "75";
        ellipse.RY = "75";
        ellipse.Fill = $"url(#grad{MainColor})";
        svg.Children.Add(ellipse);
    }
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        ISvg svg = MainGraphics!.GetMainSvg(false);
        SvgRenderClass render = new();
        DrawRegularMarblePiece(svg);
        render.RenderSvgTree(svg, builder);
        base.BuildRenderTree(builder);
    }
}