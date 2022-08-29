namespace Opetong.Core.Data;
[UseScoreboard]
public partial class OpetongPlayerItem : PlayerRummyHand<RegularRummyCard>
{
    [ScoreColumn]
    public int SetsPlayed { get; set; }
    [ScoreColumn]
    public int TotalScore { get; set; }
}