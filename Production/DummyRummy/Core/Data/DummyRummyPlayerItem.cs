namespace DummyRummy.Core.Data;
[UseScoreboard]
public partial class DummyRummyPlayerItem : PlayerRummyHand<RegularRummyCard>
{
    [ScoreColumn]
    public partial int CurrentScore { get; set; }
    [ScoreColumn]
    public partial int TotalScore { get; set; }
}