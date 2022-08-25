namespace VegasSolitaire.Core.Logic;
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
        int z = 0;
        Piles.PileList.ForEach(thisColumn =>
        {
            z++;
            for (x = 1; x <= z; x++)
            {
                var thisCard = thisCol[y];
                thisCard.IsUnknown = x != z;
                thisColumn.CardList.Add(thisCard);
                y++;
            }
        });
    }
    public override bool CanAddSingleCard(int whichOne, SolitaireCard thisCard)
    {
        if (Piles.HasCardInColumn(whichOne) == false)
        {
            return thisCard.Value == EnumRegularCardValueList.King;
        }
        var prevCard = Piles.GetLastCard(whichOne);
        return prevCard.Value.Value - 1 == thisCard.Value.Value && prevCard.Color != thisCard.Color;
    }
    public override bool CanMoveCards(int whichOne, out int lastOne)
    {
        if (PreviousSelected == -1)
        {
            throw new CustomBasicException("Cannot find out whether we can move the cards because none was selected");
        }
        lastOne = -1;
        var givList = Piles.ListGivenCards(PreviousSelected);
        TempList = givList.ListValidCardsAlternateColors();
        var thisPile = Piles.PileList[whichOne];
        SolitaireCard oldCard;
        if (thisPile.CardList.Count == 0)
        {
            oldCard = TempList.Last();
            if (oldCard.Value == EnumRegularCardValueList.King)
            {
                lastOne = TempList.Count - 1;
                return true;
            }
            return false;
        }
        oldCard = Piles.GetLastCard(whichOne);
        if (oldCard.Value == EnumRegularCardValueList.LowAce)
        {
            return false;
        }
        return TempList.CanMoveCardsAlternateColors(oldCard, ref lastOne);
    }
    public override bool CanMoveToAnotherPile(int whichOne)
    {
        return false;
    }
}