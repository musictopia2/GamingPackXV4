namespace SpiderSolitaire.Core.Logic;
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
                thisCard.IsUnknown = x != 4;
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
        lastOne = -1;
        if (PreviousSelected == -1)
        {
            throw new CustomBasicException("Cannot find out whether we can move the cards because none was selected");
        }
        TempList = ListValidCards();
        var thisPile = Piles.PileList[whichOne];
        if (thisPile.CardList.Count == 0)
        {
            lastOne = TempList.Count - 1;
            return true;
        }
        var oldCard = Piles.GetLastCard(whichOne);
        if (oldCard.Value == EnumRegularCardValueList.LowAce)
        {
            return false;
        }
        return TempList.CanMoveCardsRegardlessOfColorOrSuit(oldCard, ref lastOne);
    }
    private DeckRegularDict<SolitaireCard> ListValidCards(int pile)
    {
        int beforeSelected = PreviousSelected;
        PreviousSelected = pile;
        var output = ListValidCards();
        PreviousSelected = beforeSelected;
        return output;
    }
    private DeckRegularDict<SolitaireCard> ListValidCards()
    {
        var output = Piles.ListGivenCards(PreviousSelected);
        if (output.Count == 0)
        {
            return new DeckRegularDict<SolitaireCard>(); //decided this instead of error.
        }
        return output.ListValidCardsSameSuit();
    }
    public DeckRegularDict<SolitaireCard> MoveList()
    {
        if (TempList.Count == 0)
        {
            throw new CustomBasicException("There are no cards to move");
        }
        if (TempList.Count != 13)
        {
            throw new CustomBasicException("Must move 13 cards");
        }
        return TempList;
    }
    public override bool CanMoveToAnotherPile(int whichOne)
    {
        return false;
    }
    public void RemoveCards(int whichOne)
    {
        if (TempList.Count != 13)
        {
            throw new CustomBasicException("Must have 13 cards to remove the 13 cards");
        }
        TempList.ForEach(tempCard => Piles.RemoveSpecificCardFromColumn(whichOne, tempCard.Deck));
        if (Piles.HasCardInColumn(whichOne))
        {
            var thisCard = Piles.GetLastCard(whichOne);
            Piles.RemoveFromUnknown(thisCard);
        }
        UnselectAllColumns();
    }
    public void AddCards(DeckRegularDict<SolitaireCard> thisList)
    {
        int x = 0;
        foreach (var thisPile in Piles.PileList)
        {
            if (x == thisList.Count)
            {
                break;
            }
            var thisCard = thisList[x];
            Piles.RemoveFromUnknown(thisCard);
            thisPile.CardList.Add(thisCard);
            x++;
        }
    }
    public bool CanMoveAll(int whichOne)
    {
        var thisCol = ListValidCards(whichOne);
        EnumSuitList previousSuit = EnumSuitList.None;
        int x = 0;
        if (thisCol.Count != 13)
        {
            return false;
        }
        foreach (var thisCard in thisCol)
        {
            if (x == 0)
            {
                previousSuit = thisCard.Suit;
            }
            else if (thisCard.Suit != previousSuit)
            {
                return false;
            }
            x++;
        }
        TempList = thisCol;
        return true;
    }
}