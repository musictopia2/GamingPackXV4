namespace Rummy500.Core.Data;
[UseScoreboard]
public partial class Rummy500PlayerItem : PlayerSingleHand<RegularRummyCard>
{//anything needed is here
    [ScoreColumn]
    public int PointsPlayed { get; set; }
    [ScoreColumn]
    public int CardsPlayed { get; set; }
    [ScoreColumn]
    public int CurrentScore { get; set; }
    [ScoreColumn]
    public int TotalScore { get; set; }
}