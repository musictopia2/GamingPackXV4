namespace DealCardGame.Core.Data;
public class TradeDisplayModel
{
    public string WhoPlayerName { get; set; } = "";
    public string TradePlayerName { get; set; } = "";
    public DealCardGameCardInformation WhoReceive { get; set; } = new();
    public DealCardGameCardInformation TradeReceive { get; set; } = new();
}