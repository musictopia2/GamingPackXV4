namespace YahtzeeHandsDown.Core.Cards;
public class YahtzeeHandsDownCardInformation : SimpleDeckObject, IDeckObject, ICard, IComparable<YahtzeeHandsDownCardInformation>
{
    public EnumColor Color { get; set; }
    public int FirstValue { get; set; }
    public int SecondValue { get; set; }
    public bool IsWild { get; set; }
    public YahtzeeHandsDownCardInformation()
    {
        DefaultSize = new SizeF(55, 72);
    }
    public void Populate(int chosen)
    {
        Deck = chosen;
        int x;
        int y;
        int t;
        int z = 0;
        int ups;
        for (y = 1; y <= 3; y++) // 3 possible colors
        {
            for (x = 1; x <= 6; x++) // for the regular numbers
            {
                if (x <= 3)
                {
                    ups = 3;
                }
                else
                {
                    ups = 2;
                }
                var loopTo = ups;
                for (t = 1; t <= loopTo; t++)
                {
                    z += 1;
                    if (z == Deck)
                    {
                        FirstValue = x;
                        Color = (EnumColor)y;
                        return;
                    }
                }
            }
            for (x = 1; x <= 3; x++) // this is for the combo numbers
            {
                for (t = 1; t <= 2; t++) // this is the frequency
                {
                    z += 1;
                    if (z == Deck)
                    {
                        Color = (EnumColor)y;
                        switch (x)
                        {
                            case 1:
                                {
                                    FirstValue = 1;
                                    SecondValue = 6;
                                    break;
                                }

                            case 2:
                                {
                                    FirstValue = 2;
                                    SecondValue = 5;
                                    break;
                                }

                            case 3:
                                {
                                    FirstValue = 3;
                                    SecondValue = 4;
                                    break;
                                }

                            default:
                                {
                                    throw new CustomBasicException("Nothing found");
                                }
                        }
                        return;
                    }
                }
            }
            for (x = 1; x <= 3; x++) // for the wilds
            {
                z += 1;
                if (z == Deck)
                {
                    IsWild = true;
                    Color = (EnumColor)y;
                    return;
                }
            }
        }
        for (x = 1; x <= 6; x++) // any ones
        {
            for (y = 1; y <= 2; y++)
            {
                // for the anys
                z += 1;
                if (z == Deck)
                {
                    Color = EnumColor.Any;
                    FirstValue = x;
                    return;
                }
            }
        }
        throw new Exception("Can't find the deck " + Deck);
    }
    public void Reset() { }
    int IComparable<YahtzeeHandsDownCardInformation>.CompareTo(YahtzeeHandsDownCardInformation? other)
    {
        if (Color != other!.Color)
        {
            return Color.CompareTo(other.Color);
        }
        if (FirstValue != other.FirstValue)
        {
            return FirstValue.CompareTo(other.FirstValue);
        }
        if (SecondValue != other.SecondValue)
        {
            return SecondValue.CompareTo(other.SecondValue);
        }
        return IsWild.CompareTo(other.IsWild);
    }
    public override string GetKey()
    {
        return Guid.NewGuid().ToString();
    }
}