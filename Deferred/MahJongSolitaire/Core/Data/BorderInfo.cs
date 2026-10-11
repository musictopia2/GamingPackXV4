namespace MahJongSolitaire.Core.Data;
[Cloneable(false)]
public class BoardInfo
{
    public enum EnumBoardCategory
    {
        FarLeft = 1,
        Regular = 2,
        FarRight = 3,
        VeryTop = 4
    }
    public int Floor { get; set; }
    public int RowStart { get; set; }
    public int HowManyColumns { get; set; }
    public int FrontTaken { get; set; }
    public int BackTaken { get; set; }
    public bool Enabled { get; set; } = false;
    public DeckRegularDict<MahjongSolitaireTileInfo> TileList { get; set; } = new ();
    public EnumBoardCategory BoardCategory { get; set; } = EnumBoardCategory.Regular;
    public int ColumnStart { get; set; }
}