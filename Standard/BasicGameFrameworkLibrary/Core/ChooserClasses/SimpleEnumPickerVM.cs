namespace BasicGameFrameworkLibrary.Core.ChooserClasses;
public partial class SimpleEnumPickerVM<E> : SimpleControlObservable
    where E : IFastEnumSimple
{
    public BasicList<BasicPickerData<E>> ItemList = new(); //since its intended to work with blazor now, we don't need the collection anymore.  could change my mind though.
    public EnumAutoSelectCategory AutoSelectCategory { get; set; }
    private E? _itemChosen;
    public E? ItemChosen
    {
        get { return _itemChosen; }
        set
        {
            if (SetProperty(ref _itemChosen, value))
            {
                ItemSelectionChanged!(value);
            }
        }
    }
    public event ItemClickedEventHandler? ItemClickedAsync;
    public delegate Task ItemClickedEventHandler(E piece);
    public event ItemChangedEventHandler? ItemSelectionChanged;
    public delegate void ItemChangedEventHandler(E? piece); //can't be async since property does this.
    protected BasicList<BasicPickerData<E>> PrivateGetList()
    {
        BasicList<E> firstList = _thisChoice.GetEnumList();
        BasicList<BasicPickerData<E>> tempList = new();
        firstList.ForEach(items =>
        {
            BasicPickerData<E> thisTemp = new();
            thisTemp.EnumValue = items;
            thisTemp.IsSelected = false;
            thisTemp.IsEnabled = IsEnabled; //start with false.  to prove the problem with bindings.
            tempList.Add(thisTemp);
        });
        return tempList;
    }
    public void LoadEntireList()
    {
        var tempList = PrivateGetList();
        ItemList.ReplaceRange(tempList);
    }
    public void LoadEntireListExcludeOne(E thisEnum)
    {
        var firstList = PrivateGetList();
        firstList.KeepConditionalItems(x => x.Equals(thisEnum) == false);
        ItemList.ReplaceRange(firstList);
    }
    private readonly IEnumListClass<E> _thisChoice;
    public ControlCommand? EnumChosenCommand { get; set; }
    [Command(EnumCommandCategory.Control)]
    private async Task ProcessClickAsync(BasicPickerData<E> chosen)
    {
        if (AutoSelectCategory == EnumAutoSelectCategory.AutoEvent)
        {
            await ItemClickedAsync!.Invoke(chosen.EnumValue!);
            return;
        }
        if (AutoSelectCategory == EnumAutoSelectCategory.AutoSelect)
        {
            SelectSpecificItem(chosen.EnumValue!);
            ItemSelectionChanged!.Invoke(chosen.EnumValue);
            return;
        }
        throw new CustomBasicException("Not Supported");
    }
    public SimpleEnumPickerVM(CommandContainer container, IEnumListClass<E> thisChoice) : base(container)
    {
        _thisChoice = thisChoice;
        LoadEntireList();
        CreateCommands();
    }
    partial void CreateCommands();
    protected override bool CanEnableFirst()
    {
        return AutoSelectCategory == EnumAutoSelectCategory.AutoEvent || AutoSelectCategory == EnumAutoSelectCategory.AutoSelect;
    }
    protected override void EnableChange()
    {
        EnumChosenCommand!.ReportCanExecuteChange();
        ItemList.ForEach(x =>
        {
            x.IsEnabled = IsEnabled;
        });
    }
    public E SelectedItem(int index)
    {
        return ItemList[index].EnumValue!;
    }
    public void UnselectAll()
    {
        ItemList.ForEach(items => items.IsSelected = false);
    }
    public void SelectSpecificItem(E optionChosen)
    {
        ItemList.ForEach(items =>
        {
            if (items.EnumValue!.Equals(optionChosen) == true)
            {
                items.IsSelected = true;
            }
            else
            {
                items.IsSelected = false;
            }
        });
    }
    public void ChooseItem(E optionChosen)
    {
        ItemList.KeepConditionalItems(x => x.EnumValue!.Equals(optionChosen));
        if (ItemList.Count != 1)
        {
            throw new CustomBasicException("Did not have just one choice for option chosen.  Rethink");
        }
    }
    public E ItemToChoose()
    {
        return ItemList.GetRandomItem().EnumValue!;
    }
    protected override void PrivateEnableAlways() { }
}