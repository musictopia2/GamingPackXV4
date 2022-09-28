namespace BasicGameFrameworkLibrary.Core.SolitaireClasses.PileObservable;
public abstract class WastePilesCP : IWaste, ISerializable
{
    public WastePilesCP(CommandContainer command)
    {
        Piles = new SolitairePilesCP(command);
        Piles.DoubleClickedAsync = Piles_DoubleClickedAsync;
        Piles.ColumnClickedAsync = Piles_ColumnClickedAsync;
        _command = command;
    }
    private async Task Piles_ColumnClickedAsync(int index)
    {
        await AllPilesClickedAsync(index);
    }
    private async Task Piles_DoubleClickedAsync(int index)
    {
        await AllDoubleClickedAsync(index);
    }
    private async Task Discards_PileClickedAsync(int index, BasicPileInfo<SolitaireCard> thisPile)
    {
        await AllPilesClickedAsync(index);
    }
    private async Task AllPilesClickedAsync(int index)
    {
        if (PileSelectedAsync == null)
        {
            return;
        }
        if (CanSelectUnselectPile(index) == false)
        {
            return;
        }
        await PileSelectedAsync.Invoke(index);
    }
    private async Task AllDoubleClickedAsync(int index)
    {
        if (DoubleClickAsync == null)
        {
            return;
        }
        if (CanSelectUnselectPile(index) == false)
        {
            return;
        }
        await DoubleClickAsync.Invoke(index);
    }
    protected int PreviousSelected;
    public SolitairePilesCP Piles;
    public CustomMultiplePile? Discards;
    protected DeckRegularDict<SolitaireCard> CardList = new();
    protected bool HasDiscard;
    public int CardsNeededToBegin { get; set; }
    protected DeckRegularDict<SolitaireCard> TempList = new();
    private readonly CommandContainer _command;
    public int HowManyPiles { get; set; }
    public Func<int, Task>? PileSelectedAsync { get; set; }
    public Func<int, Task>? DoubleClickAsync { get; set; }
    protected virtual void BeforeLoadingBoard() { }
    protected virtual void AfterFirstLoad() { }
    public void FirstLoad(bool isKlondike, IDeckDict<SolitaireCard> cardList)
    {
        CardList.ReplaceRange(cardList);
        if (HasDiscard)
        {
            throw new CustomBasicException("Already shows there is a discard.  This part should not have even ran.  Find out what happened");
        }
        Piles.IsKlondike = isKlondike;
        Piles.NumberOfPiles = HowManyPiles;
        if (Piles.PileList.Count == 0)
        {
            BeforeLoadingBoard();
            Piles.LoadBoard();
        }
        AfterFirstLoad();
    }
    public void FirstLoad(int rows, int columns)
    {
        CardList.Clear();
        HasDiscard = true;
        Discards = new (_command);
        Discards.Columns = columns;
        Discards.HasText = false;
        Discards.HasFrame = true;
        Discards.Rows = rows;
        Discards.Style = EnumMultiplePilesStyleList.HasList;
        Discards.PileClickedAsync = Discards_PileClickedAsync;
        BeforeLoadingBoard();
        Discards.LoadBoard();
        AfterFirstLoad();
    }
    public virtual void ClearBoard(IDeckDict<SolitaireCard> thisCol)
    {
        if (thisCol.Count != CardsNeededToBegin)
        {
            throw new CustomBasicException($"Needs {CardsNeededToBegin} not {thisCol.Count}");
        }
        PreviousSelected = -1;
        if (HasDiscard == false)
        {
            Piles.ClearBoard();
        }
        else
        {
            Discards!.ClearBoard();
        }
    }
    public void GetUnknowns()
    {
        if (HasDiscard == false)
        {
            Piles.GetUnknownList();
        }
    }
    public void MoveCards(int whichOne, int lasts)
    {
        if (PreviousSelected == -1)
        {
            throw new CustomBasicException("Cannot move any cards because there was none selected");
        }
        if (TempList.Count == 0)
        {
            throw new CustomBasicException("There are no cards to move");
        }
        if (HasDiscard)
        {
            throw new CustomBasicException("Cannot move cards because there are no columns here to move");
        }
        int x;
        SolitaireCard thisCard;
        for (x = lasts; x >= 0; x += -1) // 0 based.  the variable being sent in is 0 based
        {
            thisCard = TempList[x];
            Piles.RemoveSpecificCardFromColumn(PreviousSelected, thisCard.Deck);
            Piles.AddCardToColumn(whichOne, thisCard);
        }
        if (Piles.HasCardInColumn(PreviousSelected) == true)
        {
            thisCard = Piles.GetLastCard(PreviousSelected);
            Piles.RemoveFromUnknown(thisCard);
        }
        AfterRemovingFromLastPile(PreviousSelected);
        PreviousSelected = -1;
        Piles.UnselectAllPiles();
    }
    protected virtual void AfterRemovingFromLastPile(int Lasts) { }
    public virtual void MoveSingleCard(int whichOne)
    {
        if (HasDiscard == false)
        {
            Piles.MoveSingleCard(PreviousSelected, whichOne);
        }
        else
        {
            Discards!.MoveSingleCard(PreviousSelected, whichOne);
        }
        PreviousSelected = -1;
    }
    public IDeckDict<SolitaireCard> GetAllCards()
    {
        if (HasDiscard == true)
        {
            return Discards!.GetCardList();
        }
        else
        {
            throw new CustomBasicException("Not sure if we need to know the card list when its not discards.  If I am wrong; then I need to rethink");
        }
    }
    public int OneSelected()
    {
        return PreviousSelected;
    }
    public void UnselectAllColumns()
    {
        if (HasDiscard == false)
        {
            Piles.UnselectAllPiles();
        }
        else
        {
            int x;
            var loopTo = Discards!.PileList!.Count;
            for (x = 1; x <= loopTo; x++)
            {
                Discards.UnselectPile(x - 1);
            }
        }
        PreviousSelected = -1;
    }
    public void SelectUnselectPile(int whichOne)
    {
        if (PreviousSelected > 0 && PreviousSelected != whichOne)
        {
            throw new CustomBasicException("Cannot select one because " + PreviousSelected + " was already selected");
        }
        if (CanSelectUnselectPile(whichOne) == false)
        {
            return;
        }
        if (PreviousSelected == whichOne)
        {
            PreviousSelected = -1;
        }
        else
        {
            PreviousSelected = whichOne;
        }
        if (HasDiscard == false)
        {
            if (Piles.HasCardInColumn(whichOne) == false)
            {
                PreviousSelected = -1;
                return;
            }
            Piles.SelectUnselectPile(whichOne);
        }
        else
        {
            if (Discards!.HasCard(whichOne) == false)
            {
                PreviousSelected = -1;
                return;
            }
            Discards.SelectUnselectSinglePile(whichOne);
        }
    }
    public bool HasCard(int whichOne)
    {
        if (HasDiscard == false)
        {
            return Piles.HasCardInColumn(whichOne);
        }
        else
        {
            return Discards!.HasCard(whichOne);
        }
    }
    public SolitaireCard GetCard()
    {
        if (PreviousSelected == -1)
        {
            throw new CustomBasicException("There is no card previously selected to choose");
        }
        return GetCard(PreviousSelected);
    }
    public virtual bool CanSelectUnselectPile(int whichOne)
    {
        return true;
    }
    public void DoubleClickColumn(int index)
    {
        PreviousSelected = index;
    }
    public virtual void RemoveSingleCard() //needed to be virtual so block eleven can do something different.
    {
        if (PreviousSelected == -1)
        {
            throw new CustomBasicException("Cannot remove a card because none has been selected");
        }
        if (HasDiscard == false)
        {
            Piles.RemoveCardFromColumn(PreviousSelected);
            Piles.UnselectAllPiles();
            SolitaireCard ThisCard;
            if (Piles.HasCardInColumn(PreviousSelected) == true)
            {
                ThisCard = Piles.GetLastCard(PreviousSelected);
                Piles.RemoveFromUnknown(ThisCard);
            }
        }
        else
        {
            Discards!.RemoveCardFromPile(PreviousSelected);
            int x;
            var loopTo = Discards!.PileList!.Count;
            for (x = 1; x <= loopTo; x++)
            {
                Discards.UnselectPile(x - 1);
            }
        }
        AfterRemovingFromLastPile(PreviousSelected);
        PreviousSelected = -1;
    }
    public void AddSingleCard(int whichOne, SolitaireCard thisCard)
    {
        thisCard.IsSelected = false;
        if (HasDiscard == false)
        {
            Piles.AddCardToColumn(whichOne, thisCard);
        }
        else
        {
            Discards!.AddCardToPile(whichOne, thisCard);
        }
    }
    public SolitaireCard GetCard(int whichOne)
    {
        if (HasDiscard == false)
        {
            return Piles.GetLastCard(whichOne);
        }
        return Discards!.GetLastCard(whichOne);
    }
    public async Task<SavedWaste> GetSavedGameAsync()
    {
        SavedWaste output = new();
        output.PreviousSelected = PreviousSelected;
        if (HasDiscard == false)
        {
            output.PileData = await Piles.GetSavedGameAsync();
        }
        else
        {
            output.PileData = await js.SerializeObjectAsync(Discards!.PileList!);
        }
        return output;
    }
    public async Task LoadGameAsync(SavedWaste gameData)
    {
        PreviousSelected = gameData.PreviousSelected;
        if (HasDiscard == false)
        {
            await Piles.LoadSavedGameAsync(gameData.PileData);
        }
        else
        {
            Discards!.PileList = await js.DeserializeObjectAsync<BasicList<BasicPileInfo<SolitaireCard>>>(gameData.PileData);
        }
        GetUnknowns();
    }
    public abstract bool CanAddSingleCard(int whichOne, SolitaireCard thisCard);
    public abstract bool CanMoveCards(int whichOne, out int lastOne);
    public abstract bool CanMoveToAnotherPile(int whichOne);
    protected void DealOutCards(IDeckDict<SolitaireCard> ThisCol)
    {
        if (HasDiscard == true)
        {
            int x = 0;
            foreach (var thisPile in Discards!.PileList!)
            {
                thisPile.ObjectList.Add(ThisCol[x]);
                x += 1;
            }
            return;
        }
        throw new CustomBasicException("Think about how to deal out cards when its not the discard piles");
    }
}