namespace BisleySolitaire.Core.Logic;
public class WastePiles : WastePilesCP
{
    public WastePiles(CommandContainer command) : base(command)
    {
    }
    public override void ClearBoard(IDeckDict<SolitaireCard> thisCol)
    {
        base.ClearBoard(thisCol);
        int x = 0;
        Discards!.PileList!.ForEach(thisPile =>
        {
            thisPile.ObjectList.Add(thisCol[x]);
            x++;
        });
        CardList.ReplaceRange(thisCol);
    }
    protected override void AfterFirstLoad()
    {
        Discards!.RemoveFirstDiscardPiles(4);
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
    public override bool CanSelectUnselectPile(int whichOne)
    {
        if (whichOne >= 35)
        {
            return true;
        }
        var newPile = whichOne + 13;
        return Discards!.HasCard(newPile) == false;
    }
    public override bool CanMoveToAnotherPile(int whichOne)
    {
        if (PreviousSelected == -1)
        {
            throw new CustomBasicException("There is no pile even selected");
        }
        var oldCard = Discards!.GetLastCard(whichOne);
        var newCard = Discards!.GetLastCard(PreviousSelected);
        if (newCard.Suit != oldCard.Suit)
        {
            return false; //can't fill it.
        }
        if (oldCard.Value.Value - 1 == newCard.Value.Value)
        {
            return true;
        }
        return oldCard.Value.Value == newCard.Value.Value - 1;
    }
}