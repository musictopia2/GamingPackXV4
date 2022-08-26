namespace MarthaSolitaire.Core.Logic;
public class WastePiles : WastePilesCP
{
    public WastePiles(CommandContainer command) : base(command)
    {
    }
    public override void ClearBoard(IDeckDict<SolitaireCard> thisCol)
    {
        base.ClearBoard(thisCol);
        int y = 0;
        4.Times(x =>
        {
            Piles.PileList.ForEach(thisPile =>
            {
                var thisCard = thisCol[y];
                thisCard.IsUnknown = x == 1 || x == 3;
                thisPile.CardList.Add(thisCard);
                y++;
            });
        });
    }
    public override bool CanAddSingleCard(int whichOne, SolitaireCard thisCard)
    {
        return false;
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
            lastOne = 0; //only last one.
            return true;
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