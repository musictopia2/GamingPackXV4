namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.YahtzeeStyleHelpers.Data;
[UseScoreboard]
public partial class YahtzeePlayerItem<D> : SimplePlayer
    where D : SimpleDice, new()
{
    [ScoreColumn]
    public partial int Points { get; set; }
    public BasicList<RowInfo> RowList { get; set; } = []; //this would be used for the scoresheets.
}