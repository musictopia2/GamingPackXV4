namespace HitTheDeck.Core.Cards;
public class HitTheDeckCardInformation : SimpleDeckObject, IDeckObject, IComparable<HitTheDeckCardInformation>
{
    public HitTheDeckCardInformation()
    {
        DefaultSize = new SizeF(55, 72);
    }
    public string Color { get; set; } = cs1.Transparent;
    public int FirstSort { get; set; }
    public EnumTypeList CardType { get; set; }
    public EnumInstructionList Instructions { get; set; }
    public bool AnyColor { get; set; }
    public int Points { get; set; }
    public int Number { get; set; }
    private static string GetColor(int value)
    {
        if (value == 1)
        {
            return cs1.Yellow;
        }
        if (value == 2)
        {
            return cs1.Blue;
        }
        if (value == 3)
        {
            return cs1.Red;
        }
        if (value == 4)
        {
            return cs1.Green;
        }
        throw new CustomBasicException("There are only 4 colors in this game");
    }
    private void PopulateCardType()
    {
        CardType = Instructions switch
        {
            EnumInstructionList.PlayNumber => EnumTypeList.Number,
            EnumInstructionList.PlayColor => EnumTypeList.Color,
            EnumInstructionList.Flip => EnumTypeList.Flip,
            EnumInstructionList.Cut => EnumTypeList.Cut,
            EnumInstructionList.RandomDraw => EnumTypeList.Draw4,
            _ => EnumTypeList.Regular,
        };
    }
    public void Populate(int chosen)
    {
        int x;
        int y;
        int z = 0;
        int q;
        for (q = 1; q <= 4; q++)
        {
            for (x = 1; x <= 5; x++)
            {
                for (y = 1; y <= 4; y++)
                {
                    z += 1;
                    if (chosen == z)
                    {
                        Deck = chosen;
                        Color = GetColor(y);
                        FirstSort = y;
                        Number = x;
                        Instructions = EnumInstructionList.None;
                        Points = Number;
                        PopulateCardType();
                        return;
                    }
                }
            }
        }
        for (q = 1; q <= 5; q++)
        {
            for (y = 1; y <= 4; y++)
            {
                z += 1;
                if (chosen == z)
                {
                    Deck = chosen;
                    Color = GetColor(y);
                    FirstSort = y;
                    Number = 0;
                    Points = 10;
                    if (q < 3)
                    {
                        Instructions = EnumInstructionList.Cut;
                    }
                    else if (q == 3)
                    {
                        Instructions = EnumInstructionList.Flip;
                    }
                    else if (q == 4)
                    {
                        Instructions = EnumInstructionList.PlayColor;
                        AnyColor = true;
                    }
                    else if (q == 5)
                    {
                        Instructions = EnumInstructionList.RandomDraw;
                        AnyColor = true;
                    }

                    PopulateCardType();
                    return;
                }
            }
        }
        for (q = 1; q <= 2; q++)
        {
            for (y = 1; y <= 5; y++)
            {
                z += 1;
                if (chosen == z)
                {
                    Deck = chosen;
                    Number = y;
                    FirstSort = 0;
                    Points = 10;
                    Instructions = EnumInstructionList.PlayNumber;
                    AnyColor = true;
                    PopulateCardType();
                    return;
                }
            }
        }
        throw new CustomBasicException("not found.  rethink");
    }
    public void Reset() { }
    int IComparable<HitTheDeckCardInformation>.CompareTo(HitTheDeckCardInformation? other)
    {
        if (FirstSort != other!.FirstSort)
        {
            return FirstSort.CompareTo(other.FirstSort);
        }
        if (Number != other.Number)
        {
            return Number.CompareTo(other.Number);
        }
        return Instructions.CompareTo(other.Instructions);
    }
    public override string GetKey()
    {
        return Guid.NewGuid().ToString();
    }
}