namespace BeleaguredCastle.Core.Logic;
public class WastePiles : WastePilesCP
{
    public WastePiles(CommandContainer command) : base(command)
    {
    }
    public override void ClearBoard(IDeckDict<SolitaireCard> thisCol)
    {
        base.ClearBoard(thisCol);
        int x;
        int y = 0;
        for (x = 1; x <= 6; x++)
        {
            foreach (var thisPile in Piles.PileList)
            {
                thisPile.TempList.Add(thisCol[y]);
                y += 1;
            }
        }
        foreach (var thisPile in Piles.PileList)
        {
            thisPile.CardList.ReplaceRange(thisPile.TempList);
            if (thisPile.CardList.Count > 6)
            {
                throw new CustomBasicException("The card list cannot be more than 6 to start with.");
            }
        }
    }
    public override bool CanAddSingleCard(int whichOne, SolitaireCard thisCard)
    {
        return false;
    }
    public override bool CanMoveCards(int whichOne, out int lastOne)
    {
        lastOne = -1; //until i figure out what else to do.
        return false;
    }
    public override bool CanMoveToAnotherPile(int whichOne)
    {
        if (PreviousSelected == -1)
        {
            throw new CustomBasicException("There was no card selected");
        }
        if (Piles.HasCardInColumn(whichOne) == false)
        {
            return true;
        }
        var oldCard = Piles.GetLastCard(whichOne);
        var newCard = Piles.GetLastCard(PreviousSelected);
        return newCard.Value.Value + 1 == oldCard.Value.Value;
    }
}