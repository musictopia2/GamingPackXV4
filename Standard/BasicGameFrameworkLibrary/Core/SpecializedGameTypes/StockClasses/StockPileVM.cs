namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.StockClasses;
public abstract class StockPileVM<D>
    where D : IDeckObject, new()
{
    public Func<Task>? StockClickedAsync { get; set; }
    public BasicMultiplePilesCP<D> StockFrame;
    public void ClearCards()
    {
        StockFrame.PileList!.Single().ObjectList.Clear();
    }
    public void AddCard(D thisCard)
    {
        StockFrame.PileList!.Single().ObjectList.Add(thisCard);
    }
    public D GetCard()
    {
        return StockFrame.PileList!.Single().ObjectList.Last();
    }
    public int CardSelected()
    {
        if (StockFrame.PileList!.Single().ObjectList.Count == 0)
        {
            return 0;
        }
        var thisCard = GetCard();
        if (thisCard.IsSelected == false)
        {
            return 0;
        }
        return thisCard.Deck;
    }
    public DeckRegularDict<D> GetStockList()
    {
        return StockFrame.PileList!.Single().ObjectList.ToRegularDeckDict();
    }
    public void RemoveCard()
    {
        StockFrame.PileList!.Single().ObjectList.RemoveLastItem();
        StockFrame.PileList!.Single().IsSelected = false;
        if (StockFrame!.PileList!.Single().ObjectList.Count > 0)
        {
            var thisCard = GetCard();
            thisCard.IsUnknown = false;
        }
    }
    public StockPileVM(CommandContainer command)
    {
        StockFrame = new(command);
        StockFrame.Style = EnumMultiplePilesStyleList.HasList;
        StockFrame.Rows = 1;
        StockFrame.Columns = 1;
        StockFrame.HasText = true;
        StockFrame.HasFrame = true;
        StockFrame.DisableWhenNoneLeft = true; //this means once no more left, then can't even enable it.
        StockFrame.LoadBoard();
        StockFrame.PileList!.Single().Text = TextToAppear;
        StockFrame.PileClickedAsync = StockFrame_PileClickedAsync;
    }
    public void UnselectCard()
    {
        if (StockFrame!.PileList!.First().ObjectList.Count == 0)
        {
            return;
        }
        D thisCard = GetCard();
        thisCard.IsSelected = false;
        StockFrame!.PileList!.Single().IsSelected = false;
    }
    public void SelectCard()
    {
        var thisCard = GetCard();
        thisCard.IsSelected = true;
        StockFrame.PileList!.Single().IsSelected = true;
    }
    public bool DidGoOut()
    {
        if (StockFrame.PileList!.Single().ObjectList.Count == 0)
        {
            return true;
        }
        return false;
    }
    public int CardsLeft()
    {
        return StockFrame!.PileList!.Single().ObjectList.Count;
    }
    public abstract string NextCardInStock();
    private async Task StockFrame_PileClickedAsync(int index, BasicPileInfo<D> thisPile)
    {
        if (StockClickedAsync == null)
        {
            return;
        }
        await StockClickedAsync.Invoke();
    }
    protected virtual string TextToAppear
    {
        get
        {
            return "Stock";
        }
    }
}