namespace Concentration.Core.Logic;
public class GameBoardClass : BasicMultiplePilesCP<RegularSimpleCard>
{
    private readonly ConcentrationGameContainer _gameContainer;
    public GameBoardClass(ConcentrationGameContainer gameContainer) : base(gameContainer.Command)
    {
        HasText = false;
        HasFrame = false;
        Rows = 5;
        Columns = 10;
        Style = EnumMultiplePilesStyleList.SingleCard;
        LoadBoard();
        _gameContainer = gameContainer;
    }
    public override void ClearBoard()
    {
        int x = 0;
        if (_gameContainer.DeckList!.Count != 50)
        {
            throw new CustomBasicException("Must have 50 cards total");
        }
        if (PileList!.Count != 50)
        {
            throw new CustomBasicException("There should have been 50 piles.");
        }
        _gameContainer.DeckList.ForEach(thisCard =>
        {
            x++;
            var thisPile = PileList[x - 1];
            thisPile.ThisObject = thisCard;
            thisPile.ThisObject.IsUnknown = true;
            thisCard.Visible = true;
            thisPile.Visible = true;
            thisPile.ThisObject.IsSelected = false;
        }); //hopefully no need for newcardprocess since that was taken out (?)
    }
    public void SelectedCardsGone()
    {
        PileList!.ForConditionalItems(items => items.IsSelected == true, thisPile =>
        {
            thisPile.IsSelected = false;
            thisPile.ThisObject.Visible = false;
        });
    }
    public void UnselectCards()
    {
        PileList!.ForConditionalItems(items => items.IsSelected == true, thisPile =>
        {
            thisPile.IsSelected = false;
            thisPile.ThisObject.IsUnknown = true;
        });
    }
    public bool CardsGone => PileList!.All(items => items.ThisObject.Visible == false);
    public DeckRegularDict<RegularSimpleCard> GetSelectedCards()
    {
        var output = PileList!.Where(items => items.IsSelected == true).Select(Items => Items.ThisObject).ToRegularDeckDict();
        if (output.Count > 2)
        {
            throw new CustomBasicException("Cannot have more than 2 selected.  Find out what happened");
        }
        return output;
    }
    private BasicPileInfo<RegularSimpleCard> FindPile(int deck)
    {
        return PileList!.Single(items => items.ThisObject.Deck == deck);
    }
    public DeckRegularDict<RegularSimpleCard> CardsLeft() => PileList!.Where(items => items.ThisObject.Visible == true && items.ThisObject.IsSelected == false).Select(xx => xx.ThisObject).ToRegularDeckDict();
    public bool WasSelected(int deck)
    {
        var thisPile = FindPile(deck);
        return thisPile.IsSelected;
    }
    public void SelectCard(int deck)
    {
        var thisPile = FindPile(deck);
        if (thisPile.ThisObject.Visible == false)
        {
            throw new CustomBasicException("Cannot select card because it was not visible");
        }
        thisPile.IsSelected = true;
        thisPile.ThisObject.IsUnknown = false;
    }
}