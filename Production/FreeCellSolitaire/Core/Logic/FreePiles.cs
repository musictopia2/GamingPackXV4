namespace FreeCellSolitaire.Core.Logic;
public class FreePiles : BasicMultiplePilesCP<SolitaireCard>
{
    public int HowManyFreeCells
    {
        get
        {
            int output = 0;
            4.Times(x =>
            {
                if (HasCard(x - 1) == false)
                {
                    output++;
                }
            });
            return output;
        }
    }
    public void ForceSelected(int index)
    {
        if (index == -1)
        {
            UnselectPile(OneSelected);
            return;
        }
        OneSelected = index;
    }
    public SolitaireCard GetCard()
    {
        if (OneSelected == -1)
        {
            throw new CustomBasicException("There is no card selected");
        }
        return GetLastCard(OneSelected);
    }
    public int OneSelected { get; private set; } = -1;
    public void SelectUnselectCard(int whichOne)
    {
        if (OneSelected > -1 && OneSelected != whichOne)
        {
            throw new CustomBasicException($"Cannot select one because {OneSelected} was already selected");
        }
        if (HasCard(whichOne) == false)
        {
            throw new CustomBasicException("There is no card to select");
        }
        if (whichOne == OneSelected)
        {
            OneSelected = -1;
        }
        else
        {
            OneSelected = whichOne;
        }
        SelectUnselectSinglePile(whichOne);
    }
    public void RemoveCard()
    {
        if (OneSelected == -1)
        {
            throw new CustomBasicException("There was no card selected to even remove");
        }
        RemoveCardFromPile(OneSelected);
        UnselectPile(OneSelected);
        OneSelected = -1;
    }
    public override void ClearBoard()
    {
        OneSelected = -1;
        base.ClearBoard();
    }
    public FreePiles(CommandContainer command) : base(command)
    {
        Columns = 4;
        Rows = 1;
        Style = EnumMultiplePilesStyleList.SingleCard;
        HasText = false;
        HasFrame = true;
        LoadBoard();
    }
}