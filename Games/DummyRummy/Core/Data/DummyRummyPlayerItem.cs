namespace DummyRummy.Core.Data;
[UseScoreboard]
public partial class DummyRummyPlayerItem : PlayerRummyHand<RegularRummyCard>
{
    [ScoreColumn]
    public int CurrentScore { get; set; }
    [ScoreColumn]
    public int TotalScore { get; set; }
}