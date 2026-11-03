namespace LifeBoardGame.Core.Cards;
public class LifeBaseCard : SimpleDeckObject, IDeckObject //can't be abstract or can't autosave cards.
{
    public LifeBaseCard()
    {
        DefaultSize = new SizeF(80, 100);
    }
    public EnumCardCategory CardCategory { get; set; }
    public void Populate(int chosen) { }
    public void Reset() { }
    public override string GetKey()
    {
        return Guid.NewGuid().ToString(); //try this way even though performance hit.
    }
}