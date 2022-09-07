namespace BasicGameFrameworkLibrary.Core.RegularDeckOfCards;
public class RegularMultiTRCard : RegularSimpleCard, ITrickCard<EnumSuitList>, IRummmyObject<EnumSuitList, EnumRegularColorList>
{
    public int Player { get; set; }
    public virtual int GetPoints => Points;
    public object CloneCard()
    {
        return MemberwiseClone();
    }
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
    public PointF Location { get; set; }
}