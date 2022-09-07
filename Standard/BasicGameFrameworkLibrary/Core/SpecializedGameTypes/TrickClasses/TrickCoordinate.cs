namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.TrickClasses;
public class TrickCoordinate
{
    public int Row { get; set; }
    public int Column { get; set; }
    public int Player { get; set; }
    public bool IsSelf { get; set; }
    public bool Visible { get; set; } = true;
    public bool PossibleDummy { get; set; }
    public string Text { get; set; } = "";
}