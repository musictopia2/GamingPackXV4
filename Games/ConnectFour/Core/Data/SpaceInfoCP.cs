namespace ConnectFour.Core.Data;

[UseStateChanged]
public partial class SpaceInfoCP : IBasicSpace
{
    public Vector Vector { get; set; }
    [StateProperty]
    public partial int Player { get; set; }
    [StateProperty]
    public partial string Color { get; set; } = cs1.Transparent;
    public bool HasImage => Player > 0;
    public void ClearSpace()
    {
        Player = 0;
        Color = cs1.Transparent;
    }
    public bool IsFilled()
    {
        return HasImage;
    }
}