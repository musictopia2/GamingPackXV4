namespace RageCardGame.Core.Data;
[UseScoreboard]
public partial class RageCardGamePlayerItem : PlayerTrick<EnumColor, RageCardGameCardInformation>
{
    [ScoreColumn]
    public partial int BidAmount { get; set; }
    [ScoreColumn]
    public partial bool RevealBid { get; set; }
    [ScoreColumn]
    public partial int CorrectlyBidded { get; set; }
    [ScoreColumn]
    public partial int ScoreRound { get; set; }
    [ScoreColumn]
    public partial int ScoreGame { get; set; }
}