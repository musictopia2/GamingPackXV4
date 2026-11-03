namespace Rook.Core.Cards;
public class RookCardInformation : SimpleDeckObject, IDeckObject, ITrickCard<EnumColorTypes>, IColorCard, IComparable<RookCardInformation>
{
    public int Player { get; set; }
    public PointF Location { get; set; }
    public int Points { get; set; }
    public string Display { get; set; } = "";
    public EnumColorTypes Color { get; set; }
    public bool IsDummy { get; set; }
    public bool IsBird { get; set; }
    public RookCardInformation()
    {
        DefaultSize = new SizeF(55, 72);
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
    public int ReadMainValue => CardValue;
    public EnumColorTypes GetColor => Color;
    public EnumColorTypes GetSuit => Color;
    public int GetPoints => Points;
    public bool IsObjectWild => false; //this has no wilds. hopefully does not even matter (?)
    public object CloneCard()
    {
        return MemberwiseClone();
    }
    public void Populate(int chosen)
    {
        Deck = chosen;
        int x;
        int y;
        int z = 0;
        int startAt;
        if (GlobalClass.Container!.PlayerList!.Count == 4)
        {
            startAt = 5;
        }
        else
        {
            startAt = 4;
        }
        for (x = 1; x <= 4; x++)
        {
            for (y = startAt; y <= 14; y++)
            {
                z += 1;
                if (z == Deck)
                {
                    Color = EnumColorTypes.FromValue(x);
                    CardValue = y;
                    if (y == 5)
                    {
                        Points = 5;
                    }
                    else if ((y == 10) | (y == 14))
                    {
                        Points = 10;
                    }
                    else
                    {
                        Points = 0;
                    }
                    return;
                }
            }
        }
        if (chosen == 41)
        {
            Color = EnumColorTypes.ZOther;
            CardValue = 15;
            Points = 20;
            IsBird = true;
            return;
        }
        throw new Exception("Sorry; cannot find the deck " + Deck);
    }
    public void Reset()
    {

    }
    int IComparable<RookCardInformation>.CompareTo(RookCardInformation? other)
    {
        if (Color != other!.Color)
        {
            return Color.CompareTo(other.Color);
        }
        return CardValue.CompareTo(other.CardValue);
    }
    public override string GetKey()
    {
        return Guid.NewGuid().ToString();
    }
}