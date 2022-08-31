namespace DutchBlitz.Core.Cards;
public class DutchBlitzCardInformation : SimpleDeckObject, IDeckObject
{
    public DutchBlitzCardInformation()
    {
        DefaultSize = new SizeF(55, 72); //this is neeeded too.
    }
    public void Populate(int chosen)
    {
        //populating the card.

    }

    public void Reset()
    {
        //anything that is needed to reset.
    }
}