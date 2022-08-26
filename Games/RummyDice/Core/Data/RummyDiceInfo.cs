namespace RummyDice.Core.Data;
public class RummyDiceInfo : IRummmyObject<EnumColorType, EnumColorType>,
    IBasicDice<int>, IGenerateDice<int>, IComparable<RummyDiceInfo>, ISelectableObject
{
    public int HeightWidth { get; set; } = 44;
    public int Value { get; set; }
    public int Index { get; set; }
    public string Display { get; set; } = "";
    public bool IsSelected { get; set; }
    public bool Visible { get; set; } = true;
    public EnumColorType Color { get; set; }
    public bool IsWild { get; set; }
    BasicList<int> IGenerateDice<int>.GetPossibleList //decided to make it twice as likely to get 11 or 12 now.  also means we don't need the extra 2 wilds anymore.
    {
        get
        {
            if (Value == 0)
            {
                //because compile problems otherwise.
            }
            BasicList<int> output = new();
            //first 44 is just as likely.
            var temps = Enumerable.Range(1, 36);
            output.AddRange(temps);
            output.AddRange(temps);
            output.AddRange(temps); //decided to make 1 to 10 most likely.  so a person can get 11 or 12 but less likely.

            temps = Enumerable.Range(37, 4);
            output.AddRange(temps);
            output.AddRange(temps);


            temps = Enumerable.Range(45, 8);

            output.AddRange(temps);

            temps = Enumerable.Range(41, 4);
            output.AddRange(temps);
            output.AddRange(temps);
            output.AddRange(temps);
            output.AddRange(temps);
            output.AddRange(temps);

            temps = Enumerable.Range(81, 12);
            output.AddRange(temps);
            output.AddRange(temps);

            temps = Enumerable.Range(81, 4);
            output.AddRange(temps);

            temps = Enumerable.Range(94, 12);
            output.AddRange(temps);

            return output;
        }
    }
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
            for (int y = 1; y <= 10; y++)
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
        //41 to 44 are the wilds
        //45 and on are 11 and 12
        //for(int x = 1; x <=4; x++)
        //{
        //    IsWild = true;
        //    Value = 0;
        //    Display = "W";
        //    return;
        //}
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
        for (int x = 1; x <= 4; x++)
        {
            for (int y = 11; y <= 12; y++)
            {
                z++;
                if (z == chosen)
                {
                    Index = chosen;
                    Color = EnumColorType.FromValue(x);
                    Value = y;
                    Display = Value.ToString();
                    IsWild = false;
                    return;
                }
            }
        }
        z = 80;
        for (int y = 1; y <= 10; y++)
        {
            for (int x = 1; x <= 4; x++)
            {
                z++;
                if (z == chosen)
                {
                    Index = chosen;
                    Color = EnumColorType.FromValue(x);
                    Value = y;
                    Display = Value.ToString();
                    IsWild = false;
                    return;

                }
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