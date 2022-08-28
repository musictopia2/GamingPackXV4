namespace Risk.Core.Cards;
public class RiskCardInfo : SimpleDeckObject, IDeckObject, IComparable<RiskCardInfo>
{
    public RiskCardInfo()
    {
        DefaultSize = new SizeF(55, 72); //maybe needed as well.
    }
    public EnumArmyType Army { get; set; }
    public void Populate(int chosen)
    {
        Army = chosen switch
        {
            0 => throw new CustomBasicException("Cannot have deck of 0"),
            < 3 => EnumArmyType.Wild,
            < 17 => EnumArmyType.Infantry,
            < 31 => EnumArmyType.Artillery,
            < 45 => EnumArmyType.Cavalry,
            _ => throw new CustomBasicException("Must be between 1 and 44")
        };
        Deck = chosen;
    }
    public void Reset()
    {

    }
    int IComparable<RiskCardInfo>.CompareTo(RiskCardInfo? other)
    {
        return Army.CompareTo(other!.Army); //any ordering in this case is fine.  keep related items together somehow
    }
}