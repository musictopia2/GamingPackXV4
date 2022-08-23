namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.TrickClasses;
public class TrickCoordinate
{
    public int Row { get; set; }
    public int Column { get; set; }
    public int Player { get; set; } // this is the id.
    public bool IsSelf { get; set; } // well see if this is needed or not.
    public bool Visible { get; set; } = true;
    public bool PossibleDummy { get; set; }
    public string Text { get; set; } = ""; // i think it needs text.  would be best to go ahead and put in the proper text
}