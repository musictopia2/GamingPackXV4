namespace BowlingDiceGame.Blazor;
public class BowlingSingleDiceBlazor : ComponentBase
{
    [Parameter]
    public SingleDiceInfo? Dice { get; set; }
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Dice == null)
        {
            return;
        }
        SvgRenderClass render = new();
        ISvg svg = new SVG();
        Rect rect = new();
        rect.Width = "50";
        rect.Height = "50";
        rect.Fill = cc.White.ToWebColor();
        svg.Children.Add(rect);
        if (Dice.DidHit == false)
        {
            Image image = new();
            image.Width = "50";
            image.Height = "50";
            image.PopulateFullExternalImage(this, "bowlingdice.svg");
            svg.Children.Add(image);
        }
        render.RenderSvgTree(svg, 0, builder);
        base.BuildRenderTree(builder);
    }
}