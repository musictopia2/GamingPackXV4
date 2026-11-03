namespace Racko.Core.Cards;
public class RackoCardInformation : SimpleDeckObject, IDeckObject, IRummmyObject<EnumSuitList, EnumRegularColorList>, IComparable<RackoCardInformation>
{
    public RackoCardInformation()
    {
        DefaultSize = new SizeF(200, 35);
    }
    int IRummmyObject<EnumSuitList, EnumRegularColorList>.GetSecondNumber => 0;
    int ISimpleValueObject<int>.ReadMainValue => Value;
    bool IWildObject.IsObjectWild => false;
    bool IIgnoreObject.IsObjectIgnored => false;
    EnumSuitList ISuitObject<EnumSuitList>.GetSuit => EnumSuitList.None;
    EnumRegularColorList IColorObject<EnumRegularColorList>.GetColor => EnumRegularColorList.None;
    public int Value { get; set; }
    public bool WillKeep { get; set; } //only for computer ai.
    public void Populate(int chosen)
    {
        Value = chosen;
        Deck = chosen;
    }
    public void Reset() { }
    int IComparable<RackoCardInformation>.CompareTo(RackoCardInformation? other)
    {
        return 0; //needed for autoresume.
    }
    public override string GetKey()
    {
        return Guid.NewGuid().ToString();
    }
}