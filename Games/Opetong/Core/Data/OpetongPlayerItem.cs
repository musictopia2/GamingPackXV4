namespace Opetong.Core.Data;
[UseScoreboard]
public partial class OpetongPlayerItem : PlayerRummyHand<RegularRummyCard>
{
    [ScoreColumn]
    public partial int SetsPlayed { get; set; }
    [ScoreColumn]
    public partial int TotalScore { get; set; }
}