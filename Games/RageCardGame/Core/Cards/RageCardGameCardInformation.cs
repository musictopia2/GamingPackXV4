namespace RageCardGame.Core.Cards;
public class RageCardGameCardInformation : SimpleDeckObject, IDeckObject, ITrickCard<EnumColor>, IComparable<RageCardGameCardInformation>
{
    public RageCardGameCardInformation()
    {
        DefaultSize = new SizeF(55, 72);
    }
    public int Player { get; set; }
    public PointF Location { get; set; }
    public EnumColor Color { get; set; }
    public int Value { get; set; }
    public EnumSpecialType SpecialType { get; set; } = EnumSpecialType.Blank;
    int ISimpleValueObject<int>.ReadMainValue => Value;
    EnumColor ISuitObject<EnumColor>.GetSuit => Color;
    public int GetPoints
    {
        get
        {

            if (SpecialType == EnumSpecialType.Mad || SpecialType == EnumSpecialType.Bonus)
            {
                return Value;
            }
            return 0;
        }
    }
    bool IWildObject.IsObjectWild => SpecialType == EnumSpecialType.Wild;
    public void Populate(int chosen)
    {
        int x;
        int y;
        int z = 0;
        for (x = 1; x <= 6; x++)
        {
            for (y = 0; y <= 15; y++)
            {
                z += 1;
                if (z == chosen)
                {
                    Color = EnumColor.FromValue(x);
                    Value = y;
                    Deck = z;
                    SpecialType = EnumSpecialType.None;
                    return;
                }
            }
        }
        for (x = 1; x <= 2; x++)
        {
            for (y = 1; y <= 4; y++)
            {
                z += 1;
                if (z == chosen)
                {
                    Deck = z;
                    Value = 0;
                    SpecialType = (EnumSpecialType)x;
                    Color = EnumColor.None;
                    return;
                }
            }
        }
        for (x = 3; x <= 5; x++)
        {
            for (y = 1; y <= 2; y++)
            {
                z += 1;
                if (z == chosen)
                {
                    Deck = z;
                    SpecialType = (EnumSpecialType)x;
                    if (SpecialType == EnumSpecialType.Mad)
                    {
                        Value = -5;
                    }
                    else if (SpecialType == EnumSpecialType.Bonus)
                    {
                        Value = 5;
                    }
                    else
                    {
                        Value = 0;
                    }
                    Color = EnumColor.None;
                    return;
                }
            }
        }
        throw new CustomBasicException("Can't find the deck " + chosen);
    }
    public void Reset() { }
    object ITrickCard<EnumColor>.CloneCard()
    {
        return MemberwiseClone();
    }
    int IComparable<RageCardGameCardInformation>.CompareTo(RageCardGameCardInformation? other)
    {
        if (Color != other!.Color)
        {
            return Color.CompareTo(other.Color);
        }
        else if (Value != other.Value)
        {
            return Value.CompareTo(other.Value);
        }
        else
        {
            return SpecialType.CompareTo(other.SpecialType);
        }
    }
    public override string GetKey()
    {
        return Guid.NewGuid().ToString();
    }
}