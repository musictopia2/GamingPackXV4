namespace CousinRummy.Core.Data;
[UseScoreboard]
public partial class CousinRummyPlayerItem : PlayerRummyHand<RegularRummyCard>
{//anything needed is here
    [ScoreColumn]
    public partial int TokensLeft { get; set; }
    [ScoreColumn]
    public partial int CurrentScore { get; set; }
    [ScoreColumn]
    public partial int TotalScore { get; set; }
    [ScoreColumn]
    public partial bool LaidDown { get; set; }
}