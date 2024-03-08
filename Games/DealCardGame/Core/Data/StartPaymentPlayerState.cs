namespace DealCardGame.Core.Data;
public class StartPaymentPlayerState
{
    public DeckRegularDict<DealCardGameCardInformation> BankedCards { get; set; } = [];


    public BasicList<SetPropertiesModel> SetData { get; set; } = [];
}