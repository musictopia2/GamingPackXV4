namespace DealCardGame.Core.Data;
public class PrivateModel : IMappable
{
    public DeckRegularDict<DealCardGameCardInformation> Payments { get; set; } = [];
    public bool NeedsPayment { get; set; }
    public StartPaymentPlayerState State { get; set; } = new();
    public RentModel RentInfo { get; set; } = new();
    public StealPropertyModel StealInfo { get; set; } = new();
    public TradePropertyModel TradeInfo { get; set; } = new();
}