namespace DealCardGame.Core.Data;
public class TradePropertyModel
{
    public int PlayerId { get; set; } //this is the player you are trading from.
    public int CardPlayed { get; set; } //this is the card played so later can be removed like normal.
    public EnumColor OpponentColor { get; set; }
    public int OpponentCard { get; set; }
    public EnumColor YourColor { get; set; }
    public int YourCard { get; set; }
    public bool StartTrading { get; set; } //this means you are starting to trade.
}