namespace MonopolyCardGame.Core.Data;
[UseScoreboard]
public partial class MonopolyCardGamePlayerItem : PlayerRummyHand<MonopolyCardGameCardInformation>
{
    [JsonIgnore]
    public TradePile? TradePile { get; set; }
    [ScoreColumn]
    public partial string TradeString { get; set; } = "";
    [ScoreColumn]
    public partial decimal PreviousMoney { get; set; }
    [ScoreColumn]
    public partial decimal TotalMoney { get; set; }
}