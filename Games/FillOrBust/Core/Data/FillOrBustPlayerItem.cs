namespace FillOrBust.Core.Data;
[UseScoreboard]
public partial class FillOrBustPlayerItem : PlayerSingleHand<FillOrBustCardInformation>
{
    [ScoreColumn]
    public int CurrentScore { get; set; }
    [ScoreColumn]
    public int TotalScore { get; set; }
}