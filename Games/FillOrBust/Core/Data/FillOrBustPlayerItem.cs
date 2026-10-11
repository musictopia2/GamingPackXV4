namespace FillOrBust.Core.Data;
[UseScoreboard]
public partial class FillOrBustPlayerItem : PlayerSingleHand<FillOrBustCardInformation>
{
    [ScoreColumn]
    public partial int CurrentScore { get; set; }
    [ScoreColumn]
    public partial int TotalScore { get; set; }
}