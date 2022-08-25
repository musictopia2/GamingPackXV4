namespace TriangleSolitaire.Core.Logic;
public class TriangleBoard : TriangleObservable
{
    public TriangleBoard(ITriangleVM thisMod, CommandContainer command) : base(thisMod, command, 5)
    {
    }
    public override void LoadSavedTriangles(SavedTriangle thisT)
    {
        base.LoadSavedTriangles(thisT);
        if (CardList.Count != 15)
        {
            throw new CustomBasicException("After loading saved game, must have 15 cards");
        }
    }
    public void ClearCards(IEnumerable<SolitaireCard> thisCol)
    {
        if (thisCol.Count() != 15)
        {
            throw new CustomBasicException("Must have 15 cards to place here");
        }
        CardList.ReplaceRange(thisCol);
        ClearBoard();
        RecalculateEnables();
    }
    public void MakeInvisible(int deck)
    {
        var thisCard = CardList.Single(items => items.Deck == deck);
        thisCard.Visible = false;
        RecalculateEnables();
    }
    public int HowManyCardsLeft => CardList.Count(items => items.Visible == true);
}