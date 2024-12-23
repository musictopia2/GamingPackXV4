namespace RummyDice.Core.Data;
public class RummyDiceInfo : IRummmyObject<EnumColorType, EnumColorType>,
    IBasicDice<int>, IComparable<RummyDiceInfo>, ISelectableObject
{
    public int HeightWidth { get; set; } = 44;
    public int Value { get; set; }
    public int Index { get; set; }
    public string Display { get; set; } = "";
    public bool IsSelected { get; set; }
    public bool Visible { get; set; } = true;
    public EnumColorType Color { get; set; }
    public bool IsWild { get; set; }
    int IRummmyObject<EnumColorType, EnumColorType>.GetSecondNumber => 0;
    int ISimpleValueObject<int>.ReadMainValue => Value;
    bool IWildObject.IsObjectWild => IsWild;
    bool IIgnoreObject.IsObjectIgnored => false;
    EnumColorType ISuitObject<EnumColorType>.GetSuit => Color;
    EnumColorType IColorObject<EnumColorType>.GetColor => Color;
    public void Populate(int chosen)
    {
        if (chosen == 0)
        {
            throw new CustomBasicException("Cannot choose 0");
        }
        int z = 0;
        for (int x = 1; x <= 4; x++)
        {
            for (int y = 1; y <= 12; y++)
            {
                z++;
                if (z == chosen)
                {
                    Index = chosen;
                    Color = EnumColorType.FromValue(x);
                    IsWild = false;
                    Value = y;
                    Display = Value.ToString();
                    return;
                }
            }
        }
        //1 to 40 is 1 to 10
        //41 to 48 is 11 and 12
        //49 to 52 are wilds.

        for (int x = 1; x <= 4; x++)
        {
            z++;
            if (z == chosen)
            {
                IsWild = true;
                Value = 0;
                Display = "W";
                Color = EnumColorType.FromValue(x);
                return;
            }
        }
        throw new CustomBasicException($"Nothing found for {chosen}");
    }
    int IComparable<RummyDiceInfo>.CompareTo(RummyDiceInfo? other) //probably important to sort the dice this time.
    {
        if (IsWild.Equals(other!.IsWild) == false)
        {
            return IsWild.CompareTo(other.IsWild);
        }
        if (Value.Equals(other.Value) == false)
        {
            return Value.CompareTo(other.Value);
        }
        return Color.CompareTo(other.Color);
    }
}