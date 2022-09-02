namespace RageCardGame.Core.Data;
[UseScoreboard]
public partial class RageCardGamePlayerItem : PlayerTrick<EnumColor, RageCardGameCardInformation>
{
    [ScoreColumn]
    public int BidAmount { get; set; }
    [ScoreColumn]
    public bool RevealBid { get; set; }
    [ScoreColumn]
    public int CorrectlyBidded { get; set; }
    [ScoreColumn]
    public int ScoreRound { get; set; }
    [ScoreColumn]
    public int ScoreGame { get; set; }
}