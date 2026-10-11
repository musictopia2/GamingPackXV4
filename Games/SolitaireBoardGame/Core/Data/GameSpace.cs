namespace SolitaireBoardGame.Core.Data;
[UseStateChanged]
public partial class GameSpace : IBasicSpace
{
    public Vector Vector { get; set; }
    [StateProperty]
    public partial bool HasImage { get; set; }
    [StateProperty]
    public partial string Color { get; set; } = cs1.Transparent;
    public void ClearSpace()
    {
        Color = cs1.Blue; //was blue.  trying green for experimenting.
    }
    public bool IsFilled()
    {
        return false; //until we figure out what we do about this one.
    }
}