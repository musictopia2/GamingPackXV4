namespace Chinazo.Core.Data;
[UseScoreboard]
public partial class ChinazoPlayerItem : PlayerRummyHand<ChinazoCard>
{
    [ScoreColumn]
    public int CurrentScore { get; set; }
    [ScoreColumn]
    public int TotalScore { get; set; }
    [ScoreColumn]
    public bool LaidDown { get; set; }
}