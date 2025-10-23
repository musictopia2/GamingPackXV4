namespace YaBlewIt.Blazor;
public class ScoreGuideComponent : ComponentBase
{
    ISvg? _mains;
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        _mains = new SVG
        {
            Height = "85vh",
            ViewBox = "0 0 2000 1200"
        };
        SvgRenderClass render = new();
        Rect rect = new();
        rect.Width = "100%";
        rect.Height = "100%";
        rect.Fill = cc1.White.ToWebColor();
        _mains.Children.Add(rect);
        DrawTop();
        DrawHeaders();
        DrawRow(250, "1X", 1);
        DrawRow(400, "2X", 3);
        DrawRow(550, "3X", 6);
        DrawRow(700, "4X", 10);
        DrawRow(850, "5X", 15);
        DrawRow(1000, "6X", 20);
        RectangleF bounds = new(20, 1100, 200, 80);
        Text text = new();
        text.Fill = cc1.Black.ToWebColor();
        text.Font_Size = 40;
        text.Content = "(or more)";
        text.CenterText(_mains, bounds);
        render.RenderSvgTree(_mains, builder);
        base.BuildRenderTree(builder);
    }
    private void DrawDottedLine(int y)
    {
        RectangleF bounds = new(550, y, 1000, 40);
        Image image = new();
        image.PopulateImagePositionings(bounds);
        image.PopulateFullExternalImage("DottedLine.svg");
        _mains!.Children.Add(image);
    }
    private void DrawHeaders()
    {
        RectangleF bounds = new(50, 120, 400, 100);
        Text text = new();
        text.Content = "Gems";
        text.Fill = cc1.Black.ToWebColor();
        text.Font_Size = 60;
        text.CenterText(_mains!, bounds);
        bounds = new(1300, 120, 600, 100);
        text = new();
        text.Content = "Points";
        text.Font_Size = 60;
        text.Fill = cc1.Black.ToWebColor();
        text.CenterText(_mains!, bounds);
        Rect line = new();
        bounds = new(25, 200, 1950, 25);
        line.PopulateRectangle(bounds);
        line.Fill = cc1.Black.ToWebColor();
        _mains!.Children.Add(line);
    }
    private void DrawTop()
    {
        RectangleF bounds = new(50, 10, 1900, 140);
        Text text = new();
        text.Content = "GEM SCORING";
        text.Fill = cc1.Black.ToWebColor();
        text.Font_Size = 120;
        text.CenterText(_mains!, bounds);
    }
    private void DrawRow(int top, string firstColumn, int points)
    {
        RectangleF bounds = new(50, top, 100, 75);
        Text text = new();
        text.Content = firstColumn;
        text.Fill = cc1.Black.ToWebColor();
        text.Font_Size = 80;
        text.CenterText(_mains!, bounds);
        bounds = new(250, top, 200, 75);
        Polygon poly = bounds.PopulateTrianglePolygon(cc1.Black);
        poly.PopulateStrokesToStyles(strokeWidth: 2);
        _mains!.Children.Add(poly);
        int others = top + 60;
        DrawDottedLine(others);
        text = new();
        bounds = new(1550, top, 200, 75);
        //has to do centered text because i am not sure how to right align it.
        text.Content = points.ToString();
        text.Fill = cc1.Black.ToWebColor();
        text.Font_Size = 80;
        text.CenterText(_mains, bounds);
    }
}