namespace BasicGameFrameworkLibrary.Blazor.GameGraphics.GamePieces;
public class PawnPiece : ComponentBase
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
            return true;
        }
        return _previousRecord!.Equals(GetRecord) == false;
    }
    [CascadingParameter]
    public BasePieceGraphics? MainGraphics { get; set; }
    [Parameter]
    public string MainColor { get; set; } = cs1.Transparent;
    protected override void OnInitialized()
    {
        MainGraphics!.OriginalSize = new SizeF(100, 100); //decided to use 150 by 150 this time.
        MainGraphics.BorderWidth = 1;
        MainGraphics.HighlightTransparent = true;
        base.OnInitialized();
    }
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        ISvg svg = MainGraphics!.GetMainSvg(false);
        svg.DrawPawnPiece(MainColor);
        SvgRenderClass render = new();
        render.RenderSvgTree(svg, builder);
        base.BuildRenderTree(builder);
    }
}