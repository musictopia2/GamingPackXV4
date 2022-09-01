namespace GalaxyCardGame.Core.Cards;
public class GalaxyCardGameCardInformation : RegularMultiTRCard, IDeckObject
{
    protected override void FinishPopulatingCard()
    {
        if (Value == EnumRegularCardValueList.HighAce)
        {
            Points = 10;
            return;
        }
        if (Value >= EnumRegularCardValueList.Ten)
        {
            Points = 10;
            return;
        }
        Points = Value.Value;
    }
}