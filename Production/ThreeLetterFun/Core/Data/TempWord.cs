namespace ThreeLetterFun.Core.Data;
public class TempWord
{
    public int CardUsed { get; set; }
    public BasicList<TilePosition> TileList { get; set; } = [];
    public int Player { get; set; }
    public int TimeToGetWord { get; set; }
}