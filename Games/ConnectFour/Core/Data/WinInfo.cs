namespace ConnectFour.Core.Data;
public class WinInfo
{
    public BasicList<SpaceInfoCP> WinList { get; set; } = [];
    public bool IsDraw { get; set; }
}