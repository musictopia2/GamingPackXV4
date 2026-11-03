namespace Bingo.Core.Data;
[UseStateChanged]
public partial class SpaceInfoCP : IBasicSpace
{
    public bool IsEnabled { get; set; } = true;
    [StateProperty]
    public partial bool AlreadyMarked { get; set; }
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