namespace PickelCardGame.Core.Data;
[UseScoreboard]
public partial class PickelCardGamePlayerItem : PlayerTrick<EnumSuitList, PickelCardGameCardInformation>
{
    [ScoreColumn]
    public EnumSuitList SuitDesired { get; set; }
    [ScoreColumn]
    public int BidAmount { get; set; }
    [ScoreColumn]
    public int CurrentScore { get; set; }
    [ScoreColumn]
    public int TotalScore { get; set; }
}