namespace Payday.Core.Cards;
public class CardInformation : SimpleDeckObject, IDeckObject
{
    public EnumCardCategory CardCategory { get; set; }
    public int Index { get; set; }
    public virtual void Populate(int chosen) { }
    public virtual void Reset() { }
    public override string GetKey()
    {
        return Guid.NewGuid().ToString();
    }
}