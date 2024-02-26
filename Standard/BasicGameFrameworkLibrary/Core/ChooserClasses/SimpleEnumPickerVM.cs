namespace BasicGameFrameworkLibrary.Core.ChooserClasses;
public partial class SimpleEnumPickerVM<E> : SimpleControlObservable
    where E : IFastEnumSimple
{
    public BasicList<BasicPickerData<E>> ItemList = new();
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
    public Func<E, Task>? ItemClickedAsync { get; set; }
    public Action<E?>? ItemSelectionChanged { get; set; }
    protected BasicList<BasicPickerData<E>> PrivateGetList()
    {
        BasicList<E> firstList = _thisChoice.GetEnumList();
        BasicList<BasicPickerData<E>> tempList = new();
        firstList.ForEach(items =>
        {
            BasicPickerData<E> thisTemp = new();
            thisTemp.EnumValue = items;
            thisTemp.IsSelected = false;
            thisTemp.IsEnabled = IsEnabled;
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
#pragma warning disable IDE0051 // Remove unused private members cannot remove because part of source generators.
    private async Task ProcessClickAsync(BasicPickerData<E> chosen)
#pragma warning restore IDE0051 // Remove unused private members
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
        //trying to not make it show the error.  hopefully does not produce any side effects.
        //if (ItemList.Count != 1)
        //{
        //    throw new CustomBasicException("Did not have just one choice for option chosen.  Rethink");
        //}
    }
    public E ItemToChoose()
    {
        return ItemList.GetRandomItem().EnumValue!;
    }
    protected override void PrivateEnableAlways() { }
}