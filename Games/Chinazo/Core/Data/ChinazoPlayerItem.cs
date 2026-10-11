namespace Chinazo.Core.Data;
[UseScoreboard]
public partial class ChinazoPlayerItem : PlayerRummyHand<ChinazoCard>
{
    [ScoreColumn]
    public partial int CurrentScore { get; set; }
    [ScoreColumn]
    public partial int TotalScore { get; set; }
    [ScoreColumn]
    public partial bool LaidDown { get; set; }
}