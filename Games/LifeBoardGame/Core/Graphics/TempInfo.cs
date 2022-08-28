namespace LifeBoardGame.Core.Graphics;
public class TempInfo
{
    public EnumViewCategory CurrentView { get; set; }
    public int HeightWidth { get; set; }
    public int CurrentNumber { get; set; }
    public BasicList<PositionInfo> PositionList { get; set; } = new(); //decided to try custom one here.  hopefully the risk pays off. otherwise, has to put old fashioned list back.
}