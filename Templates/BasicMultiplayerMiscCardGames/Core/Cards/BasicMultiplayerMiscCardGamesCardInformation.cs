namespace BasicMultiplayerMiscCardGames.Core.Cards;
public class BasicMultiplayerMiscCardGamesCardInformation : SimpleDeckObject, IDeckObject
{
    public BasicMultiplayerMiscCardGamesCardInformation()
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