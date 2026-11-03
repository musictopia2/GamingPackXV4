namespace Flinch.Core.Cards;
public class FlinchCardInformation : SimpleDeckObject, IDeckObject, IColorCard, IComparable<FlinchCardInformation>
{
    public FlinchCardInformation()
    {
        DefaultSize = new SizeF(55, 72);
    }
    public string Display { get; set; } = "";
    private int _number;
    public int Number
    {
        get { return _number; }
        set
        {
            if (SetProperty(ref _number, value))
            {
                Display = Number.ToString();
            }
        }
    }
    public EnumColorTypes Color { get; set; } = EnumColorTypes.Blue;
    int ISimpleValueObject<int>.ReadMainValue => Number;
    EnumColorTypes IColorObject<EnumColorTypes>.GetColor => EnumColorTypes.Blue; //all are blue
    public void Populate(int chosen)
    {
        int z = 0;
        for (int x = 1; x <= 15; x++)
        {
            for (int y = 1; y <= 12; y++)
            {
                z += 1;
                if (chosen == z)
                {
                    Deck = chosen;
                    Number = x;
                    return;
                }
            }
        }
        throw new CustomBasicException("Not Found.  Rethink");
    }
    public void Reset() { }
    int IComparable<FlinchCardInformation>.CompareTo(FlinchCardInformation? other)
    {
        return Number.CompareTo(other!.Number);
    }
    public override string GetKey()
    {
        return Guid.NewGuid().ToString();
    }
}