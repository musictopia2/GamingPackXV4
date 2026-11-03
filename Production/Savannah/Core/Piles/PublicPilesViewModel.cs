namespace Savannah.Core.Piles;
public class PublicPilesViewModel : BasicMultiplePilesCP<RegularSimpleCard>
{
    public bool CanClearBoard()
    {
        var list = GetCardList();
        if (list.Count != 3)
        {
            throw new CustomBasicException("There must be 3 cards to find out whether the board can be cleared or not");
        }
        return list.All(x => x.Value == EnumRegularCardValueList.LowAce);
    }
    public bool CanPlayOnPile(int pile, int numberRolled, RegularSimpleCard card)
    {
        if (card.Value == EnumRegularCardValueList.LowAce)
        {
            return true;
        }
        if (card.Value == EnumRegularCardValueList.Joker)
        {
            return true;
        }
        if (PileList![pile].ThisObject.Deck == 0)
        {
            throw new CustomBasicException("No card");
        }
        var previousCard = PileList![pile].ThisObject;
        if (previousCard.Value == EnumRegularCardValueList.King)
        {
            return true;
        }
        if (previousCard.Value == EnumRegularCardValueList.Joker)
        {
            return true;
        }
        if (previousCard.Value.Value == numberRolled)
        {
            return true;
        }
        if (previousCard.Value.Value + 1 == card.Value.Value)
        {
            return true;
        }
        return false;
    }
    public void ClearBoard(IDeckDict<RegularSimpleCard> list)
    {
        if (list.Count != 3)
        {
            throw new CustomBasicException($"The list must have 3 items, not {list.Count}");
        }
        ClearBoard();
        int x = 0;
        foreach (var card in list)
        {
            AddCardToPile(x, card);
            x++;
        }
    }
    public void FixPiles()
    {
        foreach (var pile in PileList!)
        {
            if (pile.ObjectList.Count == 1)
            {
                pile.ThisObject = pile.ObjectList.Single();
                pile.ObjectList.Clear();
            }
        }
    }
    public PublicPilesViewModel(CommandContainer command) : base(command)
    {
        Style = EnumMultiplePilesStyleList.SingleCard; //try single card now.
        Rows = 1;
        HasFrame = true;
        HasText = true;
        Columns = 3;
        LoadBoard();
        int x = 0;
        if (PileList!.Count != 3)
        {
            throw new CustomBasicException("Should have had 3 piles");
        }
        foreach (var thisPile in PileList)
        {
            x += 1;
            thisPile.Text = "Pile " + x;
        }
    }
}