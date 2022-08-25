namespace FlorentineSolitaire.Core.Logic;
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
    }
    protected override void AfterFirstLoad()
    {
        BasicList<int> thisList = new() { 0, 2, 6, 8 };
        Discards!.RemoveSpecificDiscardPiles(thisList);
    }
    public override bool CanAddSingleCard(int whichOne, SolitaireCard thisCard)
    {
        if (Discards!.HasCard(whichOne) == false)
        {
            return true;
        }
        if (whichOne == 2)
        {
            return false;
        }
        var oldCard = Discards.GetLastCard(whichOne);

        if (oldCard.Value.Value - 1 == thisCard.Value.Value)
        {
            return true;
        }
        return oldCard.Value == EnumRegularCardValueList.LowAce && thisCard.Value == EnumRegularCardValueList.King; //because round corner.
    }
    public override bool CanMoveCards(int whichOne, out int lastOne)
    {
        lastOne = -1; //until i figure out what else to do.
        return false;
    }
    public override bool CanMoveToAnotherPile(int whichOne)
    {
        if (whichOne == 2)
        {
            return false;
        }
        if (Discards!.HasCard(whichOne) == false)
        {
            return PreviousSelected == 2;
        }
        if (PreviousSelected == 2)
        {
            return false;
        }
        var oldCard = Discards.GetLastCard(whichOne);
        var newCard = Discards.GetLastCard(PreviousSelected);
        if (oldCard.Value.Value - 1 == newCard.Value.Value)
        {
            return true;
        }
        return oldCard.Value == EnumRegularCardValueList.LowAce && newCard.Value == EnumRegularCardValueList.King; //because round corner.
    }
}