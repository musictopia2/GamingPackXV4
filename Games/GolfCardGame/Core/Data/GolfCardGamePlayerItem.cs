namespace GolfCardGame.Core.Data;
[UseScoreboard]
public partial class GolfCardGamePlayerItem : PlayerSingleHand<RegularSimpleCard>
{//anything needed is here
    [ScoreColumn]
    public bool Knocked { get; set; }
    [ScoreColumn]
    public bool FirstChanged { get; set; }
    [ScoreColumn]
    public bool SecondChanged { get; set; }
    [ScoreColumn]
    public int PreviousScore { get; set; }
    [ScoreColumn]
    public int TotalScore { get; set; }
    [ScoreColumn]
    public bool FinishedChoosing { get; set; }
    public DeckRegularDict<RegularSimpleCard> TempSets { get; set; } = new();
}