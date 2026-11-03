namespace FiveCrowns.Core.Data;
[UseScoreboard]
public partial class FiveCrownsPlayerItem : PlayerRummyHand<FiveCrownsCardInformation>
{//anything needed is here
    [ScoreColumn]
    public partial int CurrentScore { get; set; }
    [ScoreColumn]
    public partial int TotalScore { get; set; }
}