namespace TeeItUp.Core.Cards;
public class TeeItUpCardInformation : SimpleDeckObject, IDeckObject, IComparable<TeeItUpCardInformation>
{
    public TeeItUpCardInformation()
    {
        DefaultSize = new SizeF(76, 105);
    }
    public int Points { get; set; } = -6;
    public bool IsMulligan { get; set; }
    public bool MulliganUsed { get; set; }
    private static int FindIndex(int chosen)
    {
        if (chosen <= 0)
        {
            throw new CustomBasicException("Cannot find index when chosen is 0");
        }
        if (chosen == 1)
        {
            return 1; //-5
        }
        if (chosen < 4)
        {
            return 2; //-3
        }
        if (chosen < 6)
        {
            return 3; //-2
        }
        if (chosen < 10)
        {
            return 4; //-1
        }
        if (chosen < 12)
        {
            return 5; //mulligan
        }
        if (chosen < 16)
        {
            return 6; //0
        }
        if (chosen < 24)
        {
            return 7; //1
        }
        if (chosen < 32)
        {
            return 8;
        }
        if (chosen < 40)
        {
            return 9;
        }
        if (chosen < 48)
        {
            return 10;
        }
        if (chosen < 56)
        {
            return 11;
        }
        if (chosen < 64)
        {
            return 12;
        }
        if (chosen < 72)
        {
            return 13;
        }
        if (chosen < 80)
        {
            return 14;
        }
        if (chosen < 88)
        {
            return 15;
        }
        if (chosen <= 100)
        {
            return 5;
        }
        return 15;
    }
    public void Populate(int chosen)
    {
        int index = FindIndex(chosen);
        Deck = chosen;
        IsMulligan = index == 5;
        if (index == 1)
        {
            Points = -5;
        }
        else if (index == 2)
        {
            Points = -3;
        }
        else if (index == 3)
        {
            Points = -2;
        }
        else if (index == 4)
        {
            Points = -1;
        }
        else if (index == 5 || index == 6)
        {
            Points = 0;
        }
        else
        {
            Points = index - 6;
        }
    }
    public void Reset()
    {
        MulliganUsed = false;
    }
    int IComparable<TeeItUpCardInformation>.CompareTo(TeeItUpCardInformation? other)
    {
        return -1; //because does not have much for sorting.
    }
    public override string GetKey()
    {
        return Guid.NewGuid().ToString();
    }
}