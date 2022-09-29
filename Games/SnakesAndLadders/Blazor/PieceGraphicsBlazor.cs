namespace SnakesAndLadders.Blazor;
public class PieceGraphicsBlazor : ComponentBase
{
    [CascadingParameter]
    public BasePieceGraphics? MainGraphics { get; set; }
    [Parameter]
    public int Index { get; set; }
    private string GetColor()
    {
        return Index switch
        {
            1 => cc.Blue,
            2 => cc.DeepPink,
            3 => cc.Orange,
            4 => cc.ForestGreen,
            _ => "",
        };
    }
    [Parameter]
    public int Number { get; set; }
    protected override void OnInitialized()
    {
        MainGraphics!.OriginalSize = new SizeF(50, 50); //decided to use 300 by 300 this time.
        MainGraphics.BorderWidth = 1;
        base.OnInitialized();
    }
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Index == 0)
        {
            return;
        }
        ISvg svg = MainGraphics!.GetMainSvg();
        string color = GetColor();
        SvgRenderClass render = new();
        Circle circle = new();
        circle.PopulateCircle(0, 0, 50, color);
        svg.Children.Add(circle);
        Text text = new();
        text.Fill = cc.White.ToWebColor();
        text.Font_Size = 40; //i think (?)
        text.PopulateStrokesToStyles(strokeWidth: 1);
        text.CenterText();
        if (Number == 0)
        {
            text.Content = Index.ToString();
        }
        else
        {
            text.Content = Number.ToString();
        }
        svg.Children.Add(text);
        render.RenderSvgTree(svg, builder);
        base.BuildRenderTree(builder);
    }
}