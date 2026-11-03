namespace YahtzeeHandsDown.Core.Cards;
public class ChanceCardInfo : SimpleDeckObject, IDeckObject
{
    public int Points { get; set; }
    public ChanceCardInfo()
    {
        DefaultSize = new SizeF(55, 72);
    }
    public void Populate(int chosen)
    {
        Deck = chosen;
        if (Deck <= 3)
        {
            Points = 1;
        }
        else if (Deck <= 6)
        {
            Points = 2;
        }
        else if (Deck <= 9)
        {
            Points = 3;
        }
        else if (Deck <= 11)
        {
            Points = 5;
        }
        else if (Deck == 12)
        {
            Points = 7;
        }
        else
        {
            throw new CustomBasicException("Can't find the chance card.  Rethink");
        }
    }
    public void Reset() { }
    public override string GetKey()
    {
        return Guid.NewGuid().ToString();
    }
}