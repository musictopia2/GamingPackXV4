namespace BasicGameFrameworkLibrary.Core.GameboardPositionHelpers;
public class GameSpace
{
    public RectangleF Area
    {
        get
        {
            return _area;
        }
        set
        {
            _area = value;
            NewArea = new byte[(int)value.Width + 1, (int)value.Height + 1];
        }
    }
    private RectangleF _area;
    internal byte[,]? NewArea { get; set; }
    public bool Enabled { get; set; } = true;
    public int Index { get; set; }
    public int Row { get; set; }
    public int Column { get; set; } // don't worry about number (some games need that as well some don't)
    public BasicList<RectangleF> ObjectList { get; set; } = new();
    public BasicList<RectangleF> PieceList { get; set; } = new();
    public BasicList<string> ColorList { get; set; } = new();
}