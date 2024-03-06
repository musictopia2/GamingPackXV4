namespace DealCardGame.Core.Cards;
public class DealCardGameCardInformation : SimpleDeckObject, IDeckObject, IComparable<DealCardGameCardInformation>
{
    public DealCardGameCardInformation()
    {
        DefaultSize = new SizeF(55, 72); //this is neeeded too.
    }
    public void Populate(int chosen)
    {
        //populating the card.
        Deck = chosen; //for now, just this until i make more progress.
    }

    public void Reset()
    {
        //anything that is needed to reset.
    }

    int IComparable<DealCardGameCardInformation>.CompareTo(DealCardGameCardInformation? other)
    {
        //for now, by deck until i make more progress.
        return Deck.CompareTo(other!.Deck);
    }
}