using System.Reflection; //not common enough
namespace Xactika.Blazor;
public static class ImageHelpers
{
    public static BasicList<PointF> GetPoints(EnumShapes shape, int howMany, PointF location, bool manually, float heightWidth)
    {
        float newLeft = 0;
        float margins;
        float mults;
        mults = heightWidth / 16;
        margins = 3 * mults; // for proportions
        if (manually == true)
        {
            if (howMany == 3)
            {
                return new()
                {
                    new PointF(location.X + margins, location.Y + margins),
                    new PointF(location.X + margins, location.Y + margins + heightWidth),
                    new PointF(location.X + margins, location.Y + margins + heightWidth * 2)
                };
            }
            else if (howMany == 1)
            {
                return new() { new PointF(location.X + margins, location.Y + margins + heightWidth) };
            }
            else
            {
                return new()
                {
                    new PointF(location.X + margins, location.Y + margins + (heightWidth / 2)),
                    new PointF(location.X + margins, location.Y + margins + (heightWidth / 2) + heightWidth)
                };
            }
        }
        float top1;
        float top2;
        float top3;
        float topFirstHalf;
        float topLastHalf;
        top1 = location.Y + margins;
        top2 = top1 + heightWidth + margins;
        top3 = top2 + heightWidth + margins;
        topFirstHalf = location.Y + margins + heightWidth / 2;
        topLastHalf = topFirstHalf + heightWidth + margins;

        if (shape == EnumShapes.Balls)
        {
            newLeft = location.X + margins;
        }
        else if (shape == EnumShapes.Cubes)
        {
            newLeft = location.X + heightWidth + margins * 2;
        }
        else if (shape == EnumShapes.Cones)
        {
            newLeft = location.X + margins + heightWidth * 2 + margins * 2;
        }
        else if (shape == EnumShapes.Stars)
        {
            newLeft = location.X + margins + heightWidth * 3 + margins * 3;
            newLeft -= 4;
        }
        if (howMany == 3)
        {
            return new() { new PointF(newLeft, top1), new PointF(newLeft, top2), new PointF(newLeft, top3) };
        }
        else if (howMany == 1)
        {
            return new() { new PointF(newLeft, top2) };
        }
        else
        {
            return new() { new PointF(newLeft, topFirstHalf), new PointF(newLeft, topLastHalf) };
        }
    }
    public static void DrawCone(this IParentGraphic container, Assembly assembly, RectangleF bounds) // done
    {
        Image image = new();
        image.PopulateFullExternalImage(assembly, "Cone.svg");
        image.PopulateImagePositionings(bounds);
        container.Children.Add(image);
    }
    public static void DrawCube(this IParentGraphic container, Assembly assembly, RectangleF bounds)
    {
        Image image = new();
        image.PopulateFullExternalImage(assembly, "Cube.svg");
        image.PopulateImagePositionings(bounds);
        container.Children.Add(image);
    }
    public static void DrawBall(this IParentGraphic container, RectangleF bounds)
    {
        Circle circle = new();
        circle.PopulateCircle(bounds, cc.Red);
        circle.PopulateStrokesToStyles();
        container.Children.Add(circle);
    }
    public static void DrawStar(this IParentContainer container, RectangleF bounds)
    {
        container.DrawStar(bounds, cc.Yellow, cc.Black, 1);
    }
}
