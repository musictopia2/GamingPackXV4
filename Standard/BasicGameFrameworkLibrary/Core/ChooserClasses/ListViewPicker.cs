namespace BasicGameFrameworkLibrary.Core.ChooserClasses;
public partial class ListViewPicker : SimpleControlObservable, IListViewPicker
{
    public readonly BasicList<ListPieceModel> TextList = new();
    public enum EnumIndexMethod
    {
        Unknown = 0,
        ZeroBased = 1,
        OneBased = 2
    }
    public enum EnumSelectionMode
    {
        SingleItem = 1,
        MultipleItems = 2
    }
    [Command(EnumCommandCategory.Control)]
    private async Task ProcessClickAsync(ListPieceModel piece)
    {
        if (SelectionMode == EnumSelectionMode.SingleItem)
        {
            SelectSpecificItem(piece.Index);
        }
        else if (piece.IsSelected)
        {
            piece.IsSelected = false;
        }
        else
        {
            piece.IsSelected = true;
        }
        if (ItemSelectedAsync == null)
        {
            return;
        }
        await ItemSelectedAsync.Invoke(piece.Index, piece.DisplayText);
    }
    partial void CreateCommands();
    public ListViewPicker(CommandContainer container, IGamePackageResolver resolver) : base(container)
    {
        _privateChoose = new(resolver)
        {
            ValueList = TextList
        };
        CreateCommands();
    }
    public EnumIndexMethod IndexMethod { get; set; }
    public EnumSelectionMode SelectionMode { get; set; } = EnumSelectionMode.SingleItem;
    public Func<int, string, Task>? ItemSelectedAsync { get; set; }
    private readonly ItemChooserClass<ListPieceModel> _privateChoose;
    public int SelectedIndex { get; set; }
    public string GetText(int index)
    {
        if (IndexMethod == EnumIndexMethod.ZeroBased)
        {
            return TextList[index].DisplayText;
        }
        if (IndexMethod == EnumIndexMethod.OneBased)
        {
            return TextList[index - 1].DisplayText;// i think
        }
        throw new CustomBasicException("Don't know the index method");
    }
    public int IndexOf(string text)
    {
        return TextList.Where(Items => Items.DisplayText == text).Single().Index;
    }
    public void LoadTextList(BasicList<string> thisList)
    {
        if (IndexMethod == EnumIndexMethod.Unknown)
        {
            throw new CustomBasicException("Must know the index method in order to continue");
        }
        BasicList<ListPieceModel> tempList = new();
        int x;
        if (IndexMethod == EnumIndexMethod.OneBased)
        {
            x = 1;
        }
        else
        {
            x = 0;
        }
        foreach (var firstText in thisList)
        {
            ListPieceModel newText = new();
            newText.Index = x;
            newText.DisplayText = firstText;
            tempList.Add(newText);
            x += 1;
        }
        TextList.ReplaceRange(tempList);
    }
    public void UnselectAll()
    {
        TextList.UnselectAllObjects();
    }
    public void SelectSpecificItem(int index)
    {
        if (SelectionMode == EnumSelectionMode.SingleItem)
        {
            foreach (var thisText in TextList)
            {
                if (thisText.Index == index)
                {
                    thisText.IsSelected = true;
                }
                else
                {
                    thisText.IsSelected = false;
                }
            }
            return;
        }
        throw new CustomBasicException("Should have used SelectSeveralItems for selecting several items");
    }
    public void ShowOnlyOneSelectedItem(string text)
    {
        if (SelectionMode == EnumSelectionMode.MultipleItems)
        {
            throw new CustomBasicException("Must have single selection for showing one selected item");
        }
        ListPieceModel thisPick = TextList.Single(Items => Items.DisplayText == text);
        TextList.ReplaceAllWithGivenItem(thisPick); //i think this is best.  so a person see's just one item.
    }
    public int Count()
    {
        return TextList.Count;
    }
    public void SelectSeveralItems(BasicList<int> thisList)
    {
        if (SelectionMode == EnumSelectionMode.SingleItem)
        {
            throw new CustomBasicException("Cannot select several items because you chose to select only one item.");
        }
        UnselectAll();
        foreach (var thisItem in thisList)
        {
            var news = (from xx in TextList
                        where xx.Index == thisItem
                        select xx).Single();
            news.IsSelected = true;
        }
    }
    public BasicList<int> GetAllSelectedItems()
    {
        if (SelectionMode == EnumSelectionMode.SingleItem)
        {
            throw new CustomBasicException("Cannot get all selected items because there was only one selected.  Try using the property SelectedIndex");
        }
        return (from xx in TextList
                where xx.IsSelected == true
                select xx.Index).ToBasicList();
    }

    public ControlCommand? ItemSelectedCommand { get; set; }
    ICustomCommand IListViewPicker.ItemSelectedCommand { get => ItemSelectedCommand!; }
    BasicList<ListPieceModel> IListViewPicker.TextList => TextList;
    protected override void EnableChange()
    {
        TextList.SetEnabled(IsEnabled);
        CommandContainer.UpdateAll();
    }
    protected override void PrivateEnableAlways() { }
    public int ItemToChoose(bool requiredToChoose = true, bool useHalf = true)
    {
        return _privateChoose.ItemToChoose(requiredToChoose, useHalf);
    }
}