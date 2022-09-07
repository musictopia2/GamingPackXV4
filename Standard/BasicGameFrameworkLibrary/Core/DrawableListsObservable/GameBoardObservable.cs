namespace BasicGameFrameworkLibrary.Core.DrawableListsObservable;
public abstract partial class GameBoardObservable<D> : IPlainObservable, IControlObservable where D : IDeckObject, new()
{
    public DeckRegularDict<D> ObjectList = new();
    public int Columns { get; set; }
    public int Rows { get; set; }
    public int MaxCards { get; set; } = 0;
    public string Text { get; set; } = "";
    public bool HasFrame { get; set; } = true;
    public bool IsEnabled { get; set; }
    public bool Visible { get; set; } = true;
    protected virtual void ChangeEnabled() { }
    public void LoadSavedGame(IDeckDict<D> list)
    {
        ObjectList.ReplaceRange(list);
    }
    public int CalculateMaxCards()
    {
        if (MaxCards == 0)
        {
            return Columns * Rows;
        }
        return MaxCards;
    }
    public D GetObject(int row, int column)
    {
        int x;
        int y;
        int z = 0;
        var loopTo = Rows;
        for (x = 1; x <= loopTo; x++)
        {
            var loopTo1 = Columns;
            for (y = 1; y <= loopTo1; y++)
            {
                if (z + 1 > ObjectList.Count)
                {
                    throw new CustomBasicException($"Somehow was out of range.  z was {z + 1} and count was {ObjectList.Count}");
                }
                if (y == column && x == row)
                {
                    return ObjectList[z];
                }
                z += 1;
            }
        }
        throw new CustomBasicException("No card at row " + row + " and column " + column);
    }
    protected (int row, int column) GetRowColumnData(D thisCard)
    {
        int x;
        int y;
        int z = 0;
        var loopTo = Rows;
        for (x = 1; x <= loopTo; x++)
        {
            var loopTo1 = Columns;
            for (y = 1; y <= loopTo1; y++)
            {
                var tempCard = ObjectList[z];
                z += 1;
                if (tempCard.Deck == thisCard.Deck)
                {
                    return (x, y);
                }
            }
        }
        throw new CustomBasicException("Can't find row/column data for Card With Deck Of " + thisCard.Deck);
    }
    public void TradeObject(int row, int column, D newObject)
    {
        var firstObject = GetObject(row, column);
        ObjectList.ReplaceItem(firstObject, newObject);
    }
    public void TradeObject(int oldDeck, D newObject)
    {
        var firstObject = ObjectList.GetSpecificItem(oldDeck);
        ObjectList.ReplaceItem(firstObject, newObject);
    }
    [Command(EnumCommandCategory.Plain, Name = nameof(ObjectCommand), Can = nameof(CanExecute))]
    protected abstract Task ClickProcessAsync(D card);
    private IBasicEnableProcess? _networkProcess;
    private Func<bool>? _customFunction;
    public void SendEnableProcesses(IBasicEnableProcess nets, Func<bool> fun)
    {
        _networkProcess = nets;
        _customFunction = fun;
    }
    bool IControlObservable.CanExecute()
    {
        if (Visible == false)
        {
            return false;
        }
        if (IsEnabled == false)
        {
            return false;
        }
        return true;
    }
    public void ReportCanExecuteChange()
    {
        if (CommandContainer.IsExecuting == true && BusyCategory == EnumCommandBusyCategory.None)
        {
            IsEnabled = false;
            return;
        }
        if (_networkProcess == null)
        {
            IsEnabled = true;
            return;
        }
        if (_networkProcess.CanEnableBasics() == false)
        {
            IsEnabled = false;
            return;
        }
        IsEnabled = _customFunction!();
    }
    public EnumCommandBusyCategory BusyCategory { get; set; } = EnumCommandBusyCategory.None;
    public CommandContainer CommandContainer;
    public PlainCommand? ObjectCommand { get; set; }
    protected virtual bool CanExecute(D card)
    {
        if (card.Visible == false)
        {
            return false;
        }
        return Visible;
    }
    public GameBoardObservable(CommandContainer container)
    {
        CommandContainer = container;
        CreateCommands(container);
        CommandContainer.AddControl(this);
    }
    partial void CreateCommands(CommandContainer container);
}