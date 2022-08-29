namespace Backgammon.Core.Graphics;
public class TriangleClass
{
    public int NumberOfTiles { get; set; }
    public int PlayerOwns { get; set; }
    public BasicList<PointF> Locations { get; set; } = new();
}