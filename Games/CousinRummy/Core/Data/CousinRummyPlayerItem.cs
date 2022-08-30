namespace CousinRummy.Core.Data;
[UseScoreboard]
public partial class CousinRummyPlayerItem : PlayerRummyHand<RegularRummyCard>
{//anything needed is here
    [ScoreColumn]
    public int TokensLeft { get; set; }
    [ScoreColumn]
    public int CurrentScore { get; set; }
    [ScoreColumn]
    public int TotalScore { get; set; }
    [ScoreColumn]
    public bool LaidDown { get; set; }
}