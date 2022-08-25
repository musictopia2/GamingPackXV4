namespace XPuzzle.Core.Data;
public class XPuzzleSpaceInfo : IBasicSpace
{
    public Vector Vector { get; set; }
    public string Text { get; set; } = "";
    public string Color { get; set; } = cs.Transparent;
    public void ClearSpace()
    {
        Color = cs.Transparent;
        Text = "";
    }
    public bool IsFilled()
    {
        return !string.IsNullOrWhiteSpace(Text);
    }
}