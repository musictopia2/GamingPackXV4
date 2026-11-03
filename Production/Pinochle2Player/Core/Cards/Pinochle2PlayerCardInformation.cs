namespace Pinochle2Player.Core.Cards;
public class Pinochle2PlayerCardInformation : RegularMultiTRCard, IDeckObject
{
    public int PinochleCardValue
    {
        get
        {
            if (Value == EnumRegularCardValueList.Nine)
            {
                return 0;
            }
            if (Value == EnumRegularCardValueList.Ten)
            {
                return 10;
            }
            if (Value == EnumRegularCardValueList.Jack)
            {
                return 2;
            }
            if (Value == EnumRegularCardValueList.Queen)
            {
                return 3;
            }
            if (Value == EnumRegularCardValueList.King)
            {
                return 4;
            }
            if (Value == EnumRegularCardValueList.HighAce)
            {
                return 11;
            }
            throw new CustomBasicException("The first number must be greater than 8");
        }
    }
}