namespace FourSuitRummy.Core.Data;
[UseScoreboard]
public partial class FourSuitRummyPlayerItem : PlayerRummyHand<RegularRummyCard>
{
    [ScoreColumn]
    public int CurrentScore { get; set; }
    [ScoreColumn]
    public int TotalScore { get; set; }
    public BasicList<SavedSet> SetList { get; set; } = new();
    [JsonIgnore]
    public MainSets? MainSets { get; set; }
}