namespace A8RoundRummy.Core.Cards;
public class A8RoundRummyCardInformation : SimpleDeckObject, IDeckObject, IComparable<A8RoundRummyCardInformation>
{
    public A8RoundRummyCardInformation()
    {
        DefaultSize = new SizeF(55, 72);
    }
    public EnumCardType CardType { get; set; } = EnumCardType.None;
    public EnumCardShape Shape { get; set; } = EnumCardShape.Blank;
    public int Value { get; set; }
    public EnumColor Color { get; set; } = EnumColor.Blank;
    public void Populate(int chosen)
    {
        int x;
        int y;
        int q;
        int r;
        int z = 0;
        for (x = 1; x <= 3; x++) // shapes
        {
            for (y = 1; y <= 7; y++) // numbers
            {
                for (r = 1; r <= 2; r++)
                {
                    for (q = 1; q <= 2; q++) // how many of each
                    {
                        z += 1;
                        if (z == chosen)
                        {
                            Deck = chosen;
                            CardType = EnumCardType.Regular;
                            Shape = (EnumCardShape)x;
                            Value = y;
                            Color = (EnumColor)r;
                            return;
                        }
                    }
                }
            }
        }
        for (x = 85; x <= 96; x++)
        {
            if (x == chosen)
            {
                Deck = chosen;
                CardType = EnumCardType.Wild;
                return;
            }
        }
        for (x = 97; x <= 100; x++)
        {
            if (x == chosen)
            {
                Deck = chosen;
                CardType = EnumCardType.Reverse;
                return;
            }
        }
        throw new Exception("Can't find the deck " + chosen);
    }
    public void Reset() { }
    int IComparable<A8RoundRummyCardInformation>.CompareTo(A8RoundRummyCardInformation? other)
    {
        if (CardType != other!.CardType)
        {
            return CardType.CompareTo(other.CardType);
        }
        if (Color != other.Color)
        {
            return Color.CompareTo(other.Color);
        }
        if (Shape != other.Shape)
        {
            return Shape.CompareTo(other.Shape);
        }
        return Value.CompareTo(other.Value);
    }
    public override string GetKey()
    {
        return Guid.NewGuid().ToString();
    }
}