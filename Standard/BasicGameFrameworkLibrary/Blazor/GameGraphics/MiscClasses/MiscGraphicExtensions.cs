using System.Reflection;
namespace BasicGameFrameworkLibrary.Blazor.GameGraphics.MiscClasses;
public static class MiscGraphicExtensions
{
    public static void DrawPawnPiece(this IParentGraphic parent, RectangleF rectangle, string customColor)
    {
        SVG svg = new();
        svg.PopulateSVGStartingPoint(rectangle);
        svg.ViewBox = "0 0 100 100";
        svg.DrawPawnPiece(customColor);
        parent.Children.Add(svg);
    }
    public static void DrawPawnPiece(this ISvg svg, string customColor)
    {
        Path path = new()
        {
            Fill = customColor.ToWebColor(),
            D = "M50 12.5L33.3333 72.7273L0 87.5L100 87.5L66.6667 72.7273L50 12.5Z"
        };
        path.PopulateStrokesToStyles();
        svg.Children.Add(path);
        Ellipse ellipse = new();
        ellipse.Fill = customColor.ToWebColor();
        ellipse.PopulateStrokesToStyles();
        ellipse.CX = "50.000004";
        ellipse.CY = "16.666666";
        ellipse.RX = "16.666668";
        ellipse.RY = "16.666666";
        svg.Children.Add(ellipse);
        Rect rect = new();
        rect.Fill = customColor.ToWebColor();
        rect.PopulateStrokesToStyles();
        rect.Y = "87.5";
        rect.Width = "100";
        rect.Height = "12.5";
        svg.Children.Add(rect);
    }
    public static void DrawDice(this IParentGraphic parent, RectangleF rectangle, int value)
    {
        Image image = new();
        Assembly current = Assembly.GetExecutingAssembly(); //needs reflection because of needing assembly for the svg.
        string name = $"Dice{value}.svg";
        image.PopulateFullExternalImage(current, name);
        image.PopulateImagePositionings(rectangle);
        parent.Children.Add(image);
    }
    public static void DrawStar(this IParentContainer parent, RectangleF bounds, string solidColor, string borderColor, float borderSize)
    {
        var svg = new SVG();
        svg.PopulateSVGStartingPoint(bounds);
        svg.ViewBox = "0 0 51 48";
        parent.Children.Add(svg);
        Path path = new();
        path.AutoIncrementElement(parent);
        path.D = "m25,1 6,17h18l-14,11 5,17-15-10-15,10 5-17-14-11h18z";
        path.Fill = solidColor.ToWebColor();
        path.PopulateStrokesToStyles(borderColor.ToWebColor(), borderSize);
        svg.Children.Add(path);
    }
    public static void DrawSmiley(this IParentContainer parent, RectangleF rectangle, string solidColor, string borderColor, string eyeColor, float borderSize) //i think
    {
        if (borderColor == "" || eyeColor == "")
        {
            throw new CustomBasicException("Must have an eye color and border color at least");
        }
        Ellipse ellipse = new();
        ellipse.AutoIncrementElement(parent);
        ellipse.PopulateEllipse(rectangle);
        if (solidColor != "")
        {
            ellipse.Fill = solidColor.ToWebColor();
        }
        ellipse.PopulateStrokesToStyles(borderColor.ToWebColor(), (int)borderSize);
        parent.Children.Add(ellipse);
        var firstPoint = new PointF(rectangle.Location.X + rectangle.Width * 0.1f, rectangle.Location.Y + rectangle.Height * 4 / 7);
        Path path = new();
        PointF secondPoint;
        PointF thirdPoint;
        secondPoint = new PointF(rectangle.Location.X + rectangle.Width / 2, rectangle.Location.Y + rectangle.Height * 1.1f);
        thirdPoint = new PointF(rectangle.Location.X + rectangle.Width * 0.9f, rectangle.Location.Y + rectangle.Height * 4 / 7);
        path.PopulateStrokesToStyles(borderColor.ToWebColor(), (int)borderSize);
        path.D = $"M {firstPoint.X} {firstPoint.Y} Q {secondPoint.X} {secondPoint.Y} {thirdPoint.X} {thirdPoint.Y}";
        parent.Children.Add(path);
        ellipse = new Ellipse();
        ellipse.AutoIncrementElement(parent);
        ellipse.PopulateEllipse(rectangle);
        parent.Children.Add(ellipse);
        var rect_Eye = new RectangleF(rectangle.Location.X + rectangle.Width / 4, rectangle.Location.Y + rectangle.Height / 4, rectangle.Width / 10, rectangle.Width / 10);
        ellipse = new Ellipse();
        ellipse.AutoIncrementElement(parent);
        ellipse.PopulateEllipse(rect_Eye);
        ellipse.Fill = eyeColor.ToWebColor();
        parent.Children.Add(ellipse);
        rect_Eye = new RectangleF(rectangle.Location.X + rectangle.Width * 3 / 4 - rectangle.Width / 10, rectangle.Location.Y + rectangle.Height / 4, rectangle.Width / 10, rectangle.Width / 10);
        ellipse = new Ellipse();
        ellipse.AutoIncrementElement(parent);
        ellipse.PopulateEllipse(rect_Eye);
        ellipse.Fill = eyeColor.ToWebColor();
        parent.Children.Add(ellipse);
    }
}