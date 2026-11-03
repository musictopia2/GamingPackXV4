namespace GolfCardGame.Core.Data;
[UseScoreboard]
public partial class GolfCardGamePlayerItem : PlayerSingleHand<RegularSimpleCard>
{//anything needed is here
    [ScoreColumn]
    public partial bool Knocked { get; set; }
    [ScoreColumn]
    public partial bool FirstChanged { get; set; }
    [ScoreColumn]
    public partial bool SecondChanged { get; set; }
    [ScoreColumn]
    public partial int PreviousScore { get; set; }
    [ScoreColumn]
    public partial int TotalScore { get; set; }
    [ScoreColumn]
    public partial bool FinishedChoosing { get; set; }
    public DeckRegularDict<RegularSimpleCard> TempSets { get; set; } = [];
}