namespace CoveredUp.Core.Logic;
public static class Extensions
{
    extension (RegularSimpleCard card)
    {
        public int Points
        {
            get
            {
                if (card.IsUnknown)
                {
                    return 0; //because not known.
                }
                if (card.Value == EnumRegularCardValueList.Jack)
                {
                    return 10;
                }
                if (card.Value == EnumRegularCardValueList.Queen)
                {
                    return 10;
                }
                if (card.Value == EnumRegularCardValueList.King)
                {
                    return 0;
                }
                if (card.Value == EnumRegularCardValueList.Joker)
                {
                    return -5;
                }
                return card.Value.Value;
            }
            
        }
        public string DisplayValue
        {
            get
            {
                if (card.IsUnknown)
                {
                    return "U"; //i think blank is fine (because not sure yet).
                }
                if (card.Value.Value <= 10)
                {
                    return card.Value.Value.ToString();
                }
                if (card.Value == EnumRegularCardValueList.Jack)
                {
                    return "Jack";
                }
                if (card.Value == EnumRegularCardValueList.Queen)
                {
                    return "Q";
                }
                if (card.Value == EnumRegularCardValueList.King)
                {
                    return "K";
                }
                return "-5"; //try -5 to represent a joker.
            }            
        }
    }
}