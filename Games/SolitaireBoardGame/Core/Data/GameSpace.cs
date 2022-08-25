namespace SolitaireBoardGame.Core.Data;
public class GameSpace : IBasicSpace
{
    public Vector Vector { get; set; }
    public bool HasImage { get; set; }
    public string Color { get; set; } = cs.Transparent;
    public void ClearSpace()
    {
        Color = cs.Blue; //was blue.  trying green for experimenting.
    }
    public bool IsFilled()
    {
        return false; //until we figure out what we do about this one.
    }
}