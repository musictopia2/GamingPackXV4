namespace Bingo.Blazor;
public class BingoSpaceBlazor : GraphicsCommand
{
    private SpaceInfoCP? _space;
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (_space == null)
        {
            return;
        }
        ISvg svg = new SVG();
        SvgRenderClass render = new();
        Rect rect = new();
        rect.Width = "100%";
        rect.Height = "100%";
        string textColor;
        float fontSize;
        if (_space.IsEnabled == true)
        {
            rect.Fill = cc1.White.ToWebColor;
            rect.PopulateStrokesToStyles(strokeWidth: 4);
            textColor = cc1.Black.ToWebColor;
            if (_space.Text == "")
            {

            }
            if (_space.Text == "Free")
            {
                fontSize = 45;
            }
            else
            {
                fontSize = 60;
            }
        }
        else
        {
            rect.Fill = cc1.Black.ToWebColor;
            rect.PopulateStrokesToStyles(color: cc1.White, 4);
            textColor = cc1.White.ToWebColor;
            fontSize = 80;
        }
        svg.Children.Add(rect);
        Text text = new();
        text.Fill = textColor;
        text.CenterText();
        text.Content = _space.Text;
        text.Font_Size = fontSize;
        svg.Children.Add(text);
        if (_space.AlreadyMarked)
        {
            Circle circle = new();
            circle.PopulateCircle(4, 4, 92, cc1.Blue, .5); //can experiement to see how transparent we make it.
            svg.Children.Add(circle);
        }
        CreateClick(svg);
        render.RenderSvgTree(svg, builder);
        base.BuildRenderTree(builder);
    }
    protected override void OnInitialized()
    {
        if (CommandParameter != null)
        {
            _space = (SpaceInfoCP)CommandParameter;
        }
    }
}