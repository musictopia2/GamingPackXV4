namespace PersianSolitaire.Core.Logic;
public class WastePiles : WastePilesCP
{
    private int DealNumber
    {
        get
        {
            return _model.DealNumber;
        }
        set
        {
            _model.DealNumber = value;
        }
    }
    private readonly ScoreModel _model;
    public WastePiles(CommandContainer command,
        ScoreModel model
        ) : base(command)
    {

        _model = model;
    }
    public override void ClearBoard(IDeckDict<SolitaireCard> thisCol)
    {
        base.ClearBoard(thisCol);
        int y = 0;
        8.Times(x =>
        {
            Piles.PileList.ForEach(thisPile =>
            {
                var thisCard = thisCol[y];
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
            lastOne = TempList.Count - 1;
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
    public void Redeal()
    {
        TempList = new DeckRegularDict<SolitaireCard>();
        if (DealNumber == 3)
        {
            throw new CustomBasicException("There are only 3 deals allowed.  Therefore, there is a problem");
        }
        var thisCol = new DeckRegularDict<SolitaireCard>();
        DealNumber++;
        Piles.PileList.ForEach(thisPile =>
        {
            thisPile.CardList.ForEach(thisCard => thisCol.Add(thisCard));
        });
        Piles.ClearBoard();
        thisCol.ShuffleList();
        int y = 0;
        8.Times(x =>
        {
            foreach (var thisPile in Piles.PileList)
            {
                if (y == thisCol.Count)
                {
                    break;
                }
                var tempCard = thisCol[y];
                thisPile.CardList.Add(tempCard);
                y++;
            }
        });
        PreviousSelected = -1;
    }
}