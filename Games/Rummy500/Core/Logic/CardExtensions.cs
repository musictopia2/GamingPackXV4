namespace Rummy500.Core.Logic;
public static class CardExtensions
{
    extension (RegularRummyCard card)
    {
        public int NegativePoints
        {
            get
            {
                //i think it should just show the minus amounts.
                if (card.Value == EnumRegularCardValueList.HighAce)
                {
                    return -15;
                }
                if (card.Value < EnumRegularCardValueList.Eight)
                {
                    return -5;
                }
                return -10;
            }
        }
    }
}