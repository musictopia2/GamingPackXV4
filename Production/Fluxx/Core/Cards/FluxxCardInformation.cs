namespace Fluxx.Core.Cards;
public class FluxxCardInformation : SimpleDeckObject, IDeckObject, IComparable<FluxxCardInformation>
{
    public FluxxCardInformation()
    {
        DefaultSize = new SizeF(73, 113);
    }
    protected override void ChangeDeck()
    {
        Index = Deck;
    }
    public virtual void Populate(int chosen)
    {
        Deck = chosen;
    }
    public void Reset()
    {

    }
    public virtual string Text()
    {
        throw new CustomBasicException("Needs to override if text is needed");
    }
    public EnumCardType CardType { get; set; }
    public string Description { get; set; } = "";
    public virtual bool IncreaseOne() => false;
    public int Index { get; set; }
    protected void PopulateDescription()
    {
        Description = FluxxGameContainer.DescriptionList[Deck - 1];
    }
    public bool CanDoCardAgain()
    {
        if (Deck == 1)
        {
            return false;
        }
        return CardType == EnumCardType.Action || CardType == EnumCardType.Rule;
    }
    int IComparable<FluxxCardInformation>.CompareTo(FluxxCardInformation? other)
    {
        return Deck.CompareTo(other!.Deck);
    }
    public override string GetKey()
    {
        return Guid.NewGuid().ToString();
    }
}