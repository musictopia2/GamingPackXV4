namespace Battleship.Blazor;
public class SpaceControlBlazor : GraphicsCommand
{
    [Parameter]
    public FieldInfoCP? Field { get; set; }
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Field == null)
        {
            return;
        }
        string color = Field.FillColor.ToWebColor;
        ISvg svg = new SVG();
        SvgRenderClass render = new();
        Rect rect = new()
        {
            Width = "50",
            Height = "50",
            Fill = color
        };
        rect.PopulateStrokesToStyles(strokeWidth: 4);
        svg.Children.Add(rect);
        if (Field.Hit == EnumWhatHit.Hit)
        {
            Image image = new();
            image.Width = "50";
            image.Height = "50";
            image.PopulateBasicExternalImage("battleshipfire.svg");
            svg.Children.Add(image);
        }
        CreateClick(svg);
        render.RenderSvgTree(svg, builder);
        base.BuildRenderTree(builder);
    }
}