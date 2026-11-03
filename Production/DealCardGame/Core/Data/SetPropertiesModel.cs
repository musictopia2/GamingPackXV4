namespace DealCardGame.Core.Data;
public class SetPropertiesModel
{
    public EnumColor Color { get; set; }
    public DeckRegularDict<DealCardGameCardInformation> Cards { get; set; } = [];
}