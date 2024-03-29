namespace MonopolyCardGame.Core.Data;
[UseScoreboard]
public partial class MonopolyCardGamePlayerItem : PlayerRummyHand<MonopolyCardGameCardInformation>
{
    [JsonIgnore]
    public TradePile? TradePile { get; set; }
    [ScoreColumn]
    public string TradeString { get; set; } = "";
    [ScoreColumn]
    public decimal PreviousMoney { get; set; }
    [ScoreColumn]
    public decimal TotalMoney { get; set; }
    //[JsonIgnore]
    //public DeckRegularDict<MonopolyCardGameCardInformation> TempHands { get; set; } = []; //this is so the other screen can be filtered.
    //[JsonIgnore]
    //public DeckRegularDict<MonopolyCardGameCardInformation> TempSets { get; set; } = new();
    //these 2 are used for the temporary stuff.
}