namespace BasicGameFrameworkLibrary.Core.RegularDeckOfCards;
public class RegularRummyCard : RegularSimpleCard, IRummmyObject<EnumSuitList, EnumRegularColorList>
{
    int IRummmyObject<EnumSuitList, EnumRegularColorList>.GetSecondNumber => SecondNumber.Value;
    bool IIgnoreObject.IsObjectIgnored => false;
    public EnumRegularCardValueList SecondNumber
    {
        get
        {
            if (Value != EnumRegularCardValueList.HighAce)
            {
                return Value;
            }
            return EnumRegularCardValueList.LowAce;
        }
    }
    public int Player { get; set; }
}