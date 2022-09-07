namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.YahtzeeStyleHelpers.Containers;
public class ScoreContainer
{
    public BasicList<DiceInformation> DiceList = new();
    public BasicList<RowInfo> RowList = new();
    public Action? StartTurn { get; set; }
}