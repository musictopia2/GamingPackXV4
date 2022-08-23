namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.YahtzeeStyleHelpers.Containers;

//this will have to be registered manually.  of course, the base class may be able to do it.
public class ScoreContainer
{
    public BasicList<DiceInformation> DiceList = new();
    public BasicList<RowInfo> RowList = new();
    public Action? StartTurn { get; set; }
}