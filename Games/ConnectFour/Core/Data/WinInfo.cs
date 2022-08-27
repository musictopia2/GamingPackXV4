namespace ConnectFour.Core.Data;
public class WinInfo
{
    public BasicList<SpaceInfoCP> WinList { get; set; } = new();
    public bool IsDraw { get; set; }
}