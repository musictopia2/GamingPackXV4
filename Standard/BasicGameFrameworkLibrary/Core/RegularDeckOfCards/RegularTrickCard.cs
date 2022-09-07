namespace BasicGameFrameworkLibrary.Core.RegularDeckOfCards;
public class RegularTrickCard : RegularSimpleCard, ITrickCard<EnumSuitList>
{
    public int Player { get; set; }
    public virtual int GetPoints => Points;
    public PointF Location { get; set; }
    public object CloneCard()
    {
        return MemberwiseClone();
    }
}