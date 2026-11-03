namespace DealCardGame.Core.Data;
public class PrivateModel : IMappable
{
    public DeckRegularDict<DealCardGameCardInformation> Payments { get; set; } = [];
    public bool NeedsPayment { get; set; }
    public RentModel RentInfo { get; set; } = new();
    public StealPropertyModel StealInfo { get; set; } = new();
    public TradePropertyModel TradeInfo { get; set; } = new();
    //no need for payment state because makes no sense to have a separate model for just one property.  the setdata is needed for more than payments though.
    public DeckRegularDict<DealCardGameCardInformation> BankedCards { get; set; } = [];
    public BasicList<SetPropertiesModel> SetData { get; set; } = [];
    public bool Organizing { get; set; } //if you are organizing, then if you go back in, then go back to that point (for wpf, won't save any of it).
    public DeckRegularDict<DealCardGameCardInformation> TemporaryCards { get; set; } = []; //this needs to hold the temporary cards.
}