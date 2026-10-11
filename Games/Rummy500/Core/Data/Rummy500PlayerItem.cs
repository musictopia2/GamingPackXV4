namespace Rummy500.Core.Data;
[UseScoreboard]
public partial class Rummy500PlayerItem : PlayerSingleHand<RegularRummyCard>
{//anything needed is here
    [ScoreColumn]
    public partial int PointsPlayed { get; set; }
    [ScoreColumn]
    public partial int CardsPlayed { get; set; }
    [ScoreColumn]
    public partial int CurrentScore { get; set; }
    [ScoreColumn]
    public partial int TotalScore { get; set; }
}