namespace FiveCrowns.Core.Data;
[UseScoreboard]
public partial class FiveCrownsPlayerItem : PlayerRummyHand<FiveCrownsCardInformation>
{//anything needed is here
    [ScoreColumn]
    public int CurrentScore { get; set; }
    [ScoreColumn]
    public int TotalScore { get; set; }
}