namespace DutchBlitz.Core.Cards;
public class DutchBlitzCardInformation : SimpleDeckObject, IDeckObject, IColorCard, IComparable<DutchBlitzCardInformation>
{
    public DutchBlitzCardInformation()
    {
        DefaultSize = new SizeF(55, 72);
    }
    public string Display { get; set; } = "";
    private EnumColorTypes _color;
    public EnumColorTypes Color
    {
        get { return _color; }
        set
        {
            if (SetProperty(ref _color, value))
            {

            }
        }
    }
    private int _cardValue;
    public int CardValue
    {
        get { return _cardValue; }
        set
        {
            if (SetProperty(ref _cardValue, value))
            {
                Display = CardValue.ToString();
            }
        }
    }
    public int Player { get; set; }
    public int ReadMainValue => CardValue;
    public EnumColorTypes GetColor => Color;
    public void Populate(int chosen)
    {
        bool doubles = DutchBlitzDeckCount.DoubleDeck;
        Deck = chosen;
        int x;
        int y;
        int z;
        int q = 0;
        int maxs;
        int r;
        if (doubles == true)
        {
            maxs = 2;
        }
        else
        {
            maxs = 1;
        }
        for (x = 1; x <= 4; x++)
        {
            var loopTo = maxs;
            for (r = 1; r <= loopTo; r++)
            {
                for (y = 1; y <= 10; y++)
                {
                    for (z = 1; z <= 4; z++)
                    {
                        q += 1;
                        if (q == Deck)
                        {
                            Color = EnumColorTypes.FromValue(x);
                            CardValue = y;
                            return;
                        }
                    }
                }
            }
        }
        throw new CustomBasicException("Sorry; cannot find the deck " + Deck);
    }
    public void Reset() { }
    int IComparable<DutchBlitzCardInformation>.CompareTo(DutchBlitzCardInformation? other)
    {
        return 0; //no sorting on this game.
    }
    public override string GetKey()
    {
        return Guid.NewGuid().ToString();
    }
}