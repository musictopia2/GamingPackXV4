namespace DealCardGame.Core.Data;
public class SetPropertiesModel
{
    public EnumColor Color { get; set; }
    public BasicList<DealCardGameCardInformation> Cards { get; set; } = [];
}