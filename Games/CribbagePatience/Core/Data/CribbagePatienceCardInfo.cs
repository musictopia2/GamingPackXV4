namespace CribbagePatience.Core.Data;
public class CribbageCard : RegularRummyCard, IComparable<CribbageCard>
{
    public bool HasUsed { get; set; }
    int IComparable<CribbageCard>.CompareTo(CribbageCard? other)
    {
        if (Value != other!.Value)
        {
            return Value.CompareTo(other.Value);
        }
        return Suit.CompareTo(other.Suit);
    }
}