namespace FourSuitRummy.Core.Data;
[UseScoreboard]
public partial class FourSuitRummyPlayerItem : PlayerRummyHand<RegularRummyCard>
{
    [ScoreColumn]
    public partial int CurrentScore { get; set; }
    [ScoreColumn]
    public partial int TotalScore { get; set; }
    public BasicList<SavedSet> SetList { get; set; } = [];
    [JsonIgnore]
    public MainSets? MainSets { get; set; }
}