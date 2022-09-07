namespace BasicGameFrameworkLibrary.Core.Dominos;
public class SimpleDominoInfo : SimpleDeckObject,
    IDominoInfo, IDeckCount, IComparable<SimpleDominoInfo>
{
    public int FirstNum { get; set; }
    public int SecondNum { get; set; }
    public PointF Location { get; set; }
    public int CurrentFirst { get; set; }
    public int CurrentSecond { get; set; }
    public SimpleDominoInfo()
    {
        DefaultSize = new SizeF(95, 31);
        CurrentFirst = -1;
        CurrentSecond = -1;
    }
    public virtual int Points => FirstNum + SecondNum;
    public virtual int HighestDomino => 6;
    public void Reset()
    {
        CurrentFirst = FirstNum;
        CurrentSecond = SecondNum;
        Rotated = false;
    }
    public void Populate(int chosen)
    {
        int y;
        int z = default;
        var loopTo = HighestDomino;
        int x;
        for (x = 0; x <= loopTo; x++)
        {
            var loopTo1 = HighestDomino;
            for (y = x; y <= loopTo1; y++)
            {
                z++;
                if (z == chosen)
                {
                    FirstNum = x;
                    SecondNum = y;
                    CurrentFirst = x;
                    CurrentSecond = y;
                    Deck = chosen;
                    return;
                }
            }
        }
        throw new CustomBasicException($"Cannot find the deck of {chosen}");
    }
    public int GetDeckCount()
    {
        int y;
        int z = default;
        var loopTo = HighestDomino;
        int x;
        for (x = 0; x <= loopTo; x++)
        {
            var loopTo1 = HighestDomino;
            for (y = x; y <= loopTo1; y++)
            {
                z += 1;
            }
        }
        if (z == 0)
        {
            throw new CustomBasicException("The deck count for dominos cannot be 0");
        }
        return z;
    }
    int IComparable<SimpleDominoInfo>.CompareTo(SimpleDominoInfo? other)
    {
        if (CurrentFirst.CompareTo(other!.CurrentFirst) != 0)
        {
            return CurrentFirst.CompareTo(other.CurrentFirst);
        }
        return CurrentSecond.CompareTo(other.CurrentSecond);
    }
}