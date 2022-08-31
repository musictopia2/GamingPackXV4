namespace Skipbo.Core.Cards;
public class SkipboCardInformation : SimpleDeckObject, IDeckObject, IColorCard, IComparable<SkipboCardInformation>
{
    public SkipboCardInformation()
    {
        DefaultSize = new SizeF(55, 72);
    }
    public string Display { get; set; } = "";
    public EnumColorTypes Color { get; set; }
    private bool _isWild;
    public bool IsWild
    {
        get { return _isWild; }
        set
        {
            if (SetProperty(ref _isWild, value))
            {
                if (value == true)
                {
                    SetWild();
                }
            }

        }
    }
    private int _number;
    public int Number
    {
        get { return _number; }
        set
        {
            if (SetProperty(ref _number, value))
            {
                if (IsWild == false)
                {
                    SetValue();
                }
                else
                {
                    Display = Number.ToString();
                }
            }
        }
    }
    private void SetValue()
    {
        if (IsWild == true)
        {
            throw new CustomBasicException("Wilds should not have set value");
        }
        if (Number == 0)
        {
            throw new CustomBasicException("Number not set and its not wild");
        }
        Display = Number.ToString();
        Color = Number switch
        {
            int _ when Number <= 4 => EnumColorTypes.Blue,
            int _ when Number <= 8 => EnumColorTypes.Green,
            int _ when Number <= 12 => EnumColorTypes.Red,
            _ => throw new CustomBasicException("Only 12 are supported"),
        };
    }
    private void SetWild()
    {
        Number = 0;
        Display = "W";
        Color = EnumColorTypes.Yellow;
    }
    int ISimpleValueObject<int>.ReadMainValue => Number;
    EnumColorTypes IColorObject<EnumColorTypes>.GetColor => Color;
    public void Populate(int chosen)
    {
        int x;
        int y;
        int z = 0;

        for (x = 1; x <= 12; x++)
        {
            for (y = 1; y <= 12; y++)
            {
                z += 1;
                if (z == chosen)
                {
                    IsWild = false;
                    Number = x;
                    Deck = chosen;
                    return;
                }
            }
        }
        for (x = 145; x <= 162; x++)
        {
            if (x == chosen)
            {
                IsWild = true;
                Deck = chosen;
                return;
            }
        }
        throw new CustomBasicException($"Sorry; cannot find the deck {chosen}");
    }
    public void Reset()
    {
        if (IsWild == true)
        {
            SetWild();
        }
    }
    int IComparable<SkipboCardInformation>.CompareTo(SkipboCardInformation? other)
    {
        return Number.CompareTo(other!.Number);
    }
    public override string GetKey()
    {
        return Guid.NewGuid().ToString();
    }
    public override BasicDeckRecordModel GetRecord => new(Deck, IsSelected, Drew, IsUnknown, IsEnabled, Visible, Display);
}