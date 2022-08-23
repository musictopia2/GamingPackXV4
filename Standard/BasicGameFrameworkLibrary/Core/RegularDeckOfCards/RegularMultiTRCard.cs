namespace BasicGameFrameworkLibrary.Core.RegularDeckOfCards;

public class RegularMultiTRCard : RegularSimpleCard, ITrickCard<EnumSuitList>, IRummmyObject<EnumSuitList, EnumRegularColorList>
{
    public int Player { get; set; }
    public virtual int GetPoints => Points; //different games can have different formulas for calculating points.
    public object CloneCard()
    {
        return MemberwiseClone(); //hopefully this simple (?)
    }
    int IRummmyObject<EnumSuitList, EnumRegularColorList>.GetSecondNumber => SecondNumber.Value; //decided that even for rummy games, it will lean towards low.  if i am wrong, rethink.  for cases there is a choice, lean towards low.
    bool IIgnoreObject.IsObjectIgnored => false;
    public EnumRegularCardValueList SecondNumber //since i use low ace, here, use there too.
    {
        get
        {
            if (Value != EnumRegularCardValueList.HighAce)
            {
                return Value;
            }
            return EnumRegularCardValueList.LowAce; //second seemed to lean towards low.
        }
    }
    public PointF Location { get; set; }
}