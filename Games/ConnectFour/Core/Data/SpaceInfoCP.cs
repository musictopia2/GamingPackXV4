namespace ConnectFour.Core.Data;
public class SpaceInfoCP : IBasicSpace
{
    public Vector Vector { get; set; }
    public int Player { get; set; }
    public string Color { get; set; } = cs.Transparent;
    public bool HasImage => Player > 0;
    public void ClearSpace()
    {
        Player = 0;
        Color = cs.Transparent;
    }
    public bool IsFilled()
    {
        return HasImage;
    }
}