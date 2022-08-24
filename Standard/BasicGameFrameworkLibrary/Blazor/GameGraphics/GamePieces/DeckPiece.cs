namespace BasicGameFrameworkLibrary.Blazor.GameGraphics.GamePieces;
internal record struct DeckRecord(bool Enabled, bool IsSelected, EnumSuitList Suit);
public class DeckPiece : ComponentBase
{
    DeckRecord? _previousRecord;
    protected override void OnAfterRender(bool firstRender)
    {
        _previousRecord = GetRecord;
        base.OnAfterRender(firstRender);
    }
    protected override bool ShouldRender()
    {
        DeckRecord current = GetRecord;
        return current.Equals(_previousRecord) == false;
    }
    private DeckRecord GetRecord => new(MainGraphics!.CustomCanDo.Invoke(), MainGraphics.IsSelected, Suit);
    [Parameter]
    public EnumSuitList Suit { get; set; }
    private string GetColor
    {
        get
        {
            if (Suit == EnumSuitList.Clubs || Suit == EnumSuitList.Spades)
            {
                return cs.Black;
            }
            if (Suit == EnumSuitList.Diamonds || Suit == EnumSuitList.Hearts)
            {
                return cs.Red;
            }
            return cs.Transparent;
        }
    }
    [CascadingParameter]
    public BasePieceGraphics? MainGraphics { get; set; }
    protected override void OnInitialized()
    {
        MainGraphics!.OriginalSize = new SizeF(100, 100); //easiest just to use 100 by 100.  of course, its flexible anyways.
        MainGraphics.NeedsHighlighting = true;
        base.OnInitialized();
    }
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Suit == EnumSuitList.None)
        {
            return;
        }
        ISvg svg = MainGraphics!.GetMainSvg(false);
        SvgRenderClass render = new();
        svg.DrawCardSuit(Suit, 0, 0, 100, 100, GetColor);
        render.RenderSvgTree(svg, 0, builder);
        base.BuildRenderTree(builder);
    }
}