namespace Bingo.Core.Data;
public class SpaceInfoCP : IBasicSpace
{
    public bool IsEnabled { get; set; } = true;
    public bool AlreadyMarked { get; set; }
    public string Text { get; set; } = "";
    public Vector Vector { get; set; }
    public void ClearSpace()
    {
        AlreadyMarked = false;
    }
    public bool IsFilled()
    {
        return AlreadyMarked;
    }
}