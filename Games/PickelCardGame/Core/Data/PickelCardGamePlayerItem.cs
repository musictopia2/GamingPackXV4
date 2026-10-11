namespace PickelCardGame.Core.Data;
[UseScoreboard]
public partial class PickelCardGamePlayerItem : PlayerTrick<EnumSuitList, PickelCardGameCardInformation>
{
    [ScoreColumn]
    public partial EnumSuitList SuitDesired { get; set; }
    [ScoreColumn]
    public partial int BidAmount { get; set; }
    [ScoreColumn]
    public partial int CurrentScore { get; set; }
    [ScoreColumn]
    public partial int TotalScore { get; set; }
}