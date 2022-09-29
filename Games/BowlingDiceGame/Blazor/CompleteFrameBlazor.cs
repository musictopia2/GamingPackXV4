namespace BowlingDiceGame.Blazor;
public class CompleteFrameBlazor : ComponentBase
{
    [Parameter]
    public FrameInfoCP? Frame { get; set; }
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Frame == null || Frame.SectionList.Count != 3)
        {
            return;
        }
        SvgRenderClass render = new();
        render.Allow0 = true;
        ISvg svg = new SVG();
        svg.Width = "150";
        svg.Height = "100";
        int borderSize = 4;
        float x = 0;
        Rect rect;
        foreach (var section in Frame.SectionList)
        {
            rect = new()
            {
                Width = "50",
                Height = "50",
                X = x.ToString()
            };
            rect.PopulateStrokesToStyles(cc.White.ToWebColor(), borderSize);
            svg.Children.Add(rect);
            RectangleF r = new(x, 0, 50, 50);
            svg.DrawCenteredText(r, 20, section.Value.Score, cc.White);
            x += 50;
        }
        RectangleF f = new(0, 50, 150, 50);
        rect = new()
        {
            Width = "150",
            Height = "50",
            X = f.X.ToString(),
            Y = f.Y.ToString()
        };
        rect.PopulateStrokesToStyles(cc.White.ToWebColor(), borderSize);
        svg.Children.Add(rect);
        string score;
        if (Frame.Score == -1)
        {
            score = "";
        }
        else
        {
            score = Frame.Score.ToString();
        }
        svg.DrawCenteredText(f, 20, score, cc.White);
        render.RenderSvgTree(svg, builder);
        base.BuildRenderTree(builder);
    }
}
