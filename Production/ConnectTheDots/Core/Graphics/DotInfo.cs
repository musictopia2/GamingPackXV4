namespace ConnectTheDots.Core.Graphics;
public class DotInfo
{
    public RectangleF Dot { get; set; }
    public int Column { get; set; }
    public int Row { get; set; }
    public bool IsSelected { get; set; }
    public RectangleF Bounds { get; set; }
}