namespace BasicGameFrameworkLibrary.Core.SolitaireClasses.GraphicsObservable;
public partial class SolitairePilesCP : IPlainObservable, ISerializable
{
    #region "Variables"
    private readonly DeckRegularDict<SolitaireCard> _listUnknownCards = new();
    public BasicList<PileInfoCP> PileList = new();
    #endregion
    #region "Properties"
    /// <summary>
    /// games like persian solitaire needs this to set the proper key.
    /// </summary>
    public static int DealNumber { get; set; }
    public bool IsKlondike { get; set; }
    public int NumberOfPiles { get; set; }
    #endregion
    #region "Events/Commands"
    public event WastePileSelectedEventHandler? ColumnClickedAsync;
    public event WasteDoubleClickEventHandler? DoubleClickedAsync;
    public PlainCommand? ColumnCommand { get; set; }
    public PlainCommand? DoubleCommand { get; set; }
    [Command(EnumCommandCategory.Plain, Name = nameof(ColumnCommand))]
    private async Task PrivateColumnAsync(PileInfoCP pile)
    {
        if (ColumnClickedAsync == null)
        {
            return;
        }
        await ColumnClickedAsync.Invoke(PileList.IndexOf(pile));
    }
    [Command(EnumCommandCategory.Plain, Name = nameof(DoubleCommand))]
    private async Task PrivateDoubleAsync(PileInfoCP pile)
    {
        if (DoubleClickedAsync == null)
        {
            return;
        }
        await DoubleClickedAsync.Invoke(PileList.IndexOf(pile));
    }
    public SolitairePilesCP(CommandContainer command)
    {
        CreateCommands(command);
    }
    partial void CreateCommands(CommandContainer container);
    #endregion
    #region "Methods/Functions"
    public void SelectUnselectPile(int pile)
    {
        var thisPile = PileList[pile];
        if (thisPile.IsSelected)
        {
            thisPile.IsSelected = false;
            return;
        }
        PileList.ForEach(tempPile => tempPile.IsSelected = false);
        thisPile.IsSelected = true;
    }
    public int SinglePileSelected()
    {
        int counts = PileList.Count(items => items.IsSelected);
        if (counts == 0)
        {
            return -1;
        }
        if (counts > 1)
        {
            throw new CustomBasicException("More than one pile is selected.  Find out what happened");
        }
        var tempPile = PileList.Single(items => items.IsSelected);
        return PileList.IndexOf(tempPile);
    }
    public DeckRegularDict<SolitaireCard> ListGivenCards(int pile)
    {
        var thisPile = PileList[pile];
        DeckRegularDict<SolitaireCard> output = new();
        var thisTemp = thisPile.CardList.ToRegularDeckDict();
        thisTemp.Reverse();
        foreach (var thisCard in thisTemp)
        {
            if (thisCard.IsUnknown == false)
            {
                output.Add(thisCard);
            }
            else
            {
                break;
            }
        }
        output.Reverse();
        return output;
    }
    public virtual SolitaireCard GetLastCard(int column)
    {
        if (PileList[column].CardList.Count == 0)
        {
            throw new CustomBasicException("There are no cards to get");
        }
        return PileList[column].CardList.Last();
    }
    public virtual void RemoveCardFromColumn(int index)
    {
        if (PileList[index].CardList.Count == 0)
        {
            throw new CustomBasicException("There is no card to remove from column");
        }
        PileList[index].CardList.RemoveLastItem();
    }
    public void UnselectAllPiles()
    {
        PileList.ForEach(thisPile => thisPile.IsSelected = false);
    }
    public virtual void MoveSingleCard(int previousPile, int newPile)
    {
        var thisCard = GetLastCard(previousPile);
        RemoveCardFromColumn(previousPile);
        var thisPile = PileList[newPile];
        thisPile.CardList.Add(thisCard);
        UnselectAllPiles();
    }
    public void RemoveSpecificCardFromColumn(int pile, int deck)
    {
        PileList[pile].CardList.RemoveObjectByDeck(deck);
    }
    public void RemoveFromUnknown(SolitaireCard thisCard)
    {
        _listUnknownCards.RemoveSpecificItem(thisCard);
        thisCard.IsUnknown = false;
    }
    public virtual void AddCardToColumn(int column, SolitaireCard thisCard)
    {
        PileList[column].CardList.Add(thisCard);
    }
    public bool HasCardInColumn(int column)
    {
        return PileList[column].CardList.Count > 0;
    }
    public async Task<string> GetSavedGameAsync()
    {
        string output = await js.SerializeObjectAsync(PileList);
        return output;
    }
    public async Task LoadSavedGameAsync(string data)
    {
        PileList = await js.DeserializeObjectAsync<BasicList<PileInfoCP>>(data);
    }
    public virtual void LoadBoard()
    {
        if (NumberOfPiles == 0)
        {
            throw new CustomBasicException("Must have at least 1 pile");
        }
        NumberOfPiles.Times(x =>
        {
            PileInfoCP thisPile = new();
            PileList.Add(thisPile);
        });
    }
    public virtual void ClearBoard()
    {
        _listUnknownCards.Clear();
        PileList.ForEach(thisPile =>
        {
            thisPile.CardList.Clear();
            thisPile.TempList.Clear();
            thisPile.IsSelected = false;
        });
    }
    public void GetUnknownList()
    {
        _listUnknownCards.Clear();
        PileList.ForEach(thisPile =>
        {
            thisPile.CardList.ForConditionalItems(items => items.IsUnknown == true, thisCard => _listUnknownCards.Add(thisCard));
        });
    }
    public void MakeUnknown()
    {
        if (_listUnknownCards.Count == 0)
        {
            return;
        }
        _listUnknownCards.ForEach(thisCard => thisCard.IsUnknown = true);
    }
    #endregion
}