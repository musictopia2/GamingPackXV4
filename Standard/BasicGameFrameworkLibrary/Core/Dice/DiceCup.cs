namespace BasicGameFrameworkLibrary.Core.Dice;
public partial class DiceCup<D> : SimpleControlObservable, IRollMultipleDice<D> where D :
    IStandardDice, new()
{
    public IGamePackageResolver? MainContainer { get; set; }
    private IAsyncDelayer? _delay;
    public Func<D, Task>? DiceClickedAsync { get; set; }
    public string CommandActionString { get; set; } = "dicecup";
    public bool Visible { get; set; }
    [Command(EnumCommandCategory.Control)]
    private async Task PrivateDiceClickAsync(D dice)
    {
        if (DiceClickedAsync == null)
        {
            return;
        }
        await DiceClickedAsync.Invoke(dice);
    }
    public ControlCommand DiceCommand { get; set; }
    public DiceCup(IGamePackageResolver resolver, CommandContainer command) : base(command)
    {
        DiceList = new();
        Init(resolver);
        if (DiceCommand is null)
        {
            throw new CustomBasicException("DiceCommand cannot be null");
        }
    }
    public DiceCup(DiceList<D> privateList, IGamePackageResolver resolver, CommandContainer command) : base(command)
    {
        DiceList = privateList;
        Init(resolver);
        if (DiceCommand is null)
        {
            throw new CustomBasicException("DiceCommand cannot be null");
        }
    }
    private IGameNetwork? _network;
    private void Init(IGamePackageResolver resolver)
    {
        BasicData thisData = resolver.Resolve<BasicData>();
        MainContainer = resolver;
        DiceList.MainContainer = MainContainer;
        CreateCommands();
        if (thisData.MultiPlayer == true)
        {
            _network = resolver.Resolve<IGameNetwork>();
        }
    }
    partial void CreateCommands();
    public bool CanShowDice { get; set; }
    public int TotalDiceValue => DiceList.Sum(Items => Items.Value);
    public DiceList<D> DiceList { get; }
    int _originalNumber;
    private int _howManyDice;
    public int HowManyDice
    {
        get { return _howManyDice; }
        set
        {
            if (SetProperty(ref _howManyDice, value))
            {
                if (value > _originalNumber)
                {
                    _originalNumber = value;
                }
            }
        }
    }
    public bool HasDice { get; set; }
    public bool ShowDiceListAlways { get; set; }
    public bool ShowHold { get; set; }
    public void SelectUnselectDice(int index)
    {
        if (ShowHold == true)
        {
            throw new CustomBasicException("The dice is being held, not selected");
        }
        D thisDice = DiceList.Single(Items => Items.Index == index);
        thisDice.IsSelected = !thisDice.IsSelected;
    }
    public void HoldUnholdDice(int index)
    {
        if (ShowHold == false)
        {
            throw new CustomBasicException("The dice is being selected, not held");
        }
        D thisDice = DiceList[index - 1];
        thisDice.Hold = !thisDice.Hold;
    }
    public void UnholdDice() => DiceList.ForEach(items => items.Hold = false);
    public bool IsDiceHeld(int index)
    {
        if (ShowHold == false)
        {
            throw new CustomBasicException("The dice is being selected, not held");
        }
        return DiceList[index].Hold;
    }
    public int HowManyHeldDice()
    {
        if (ShowHold == false)
        {
            throw new CustomBasicException("The dice is being selected, not held");
        }
        return DiceList.Count(items => items.Hold);
    }
    public bool HasSelectedDice()
        => DiceList.Exists(items => items.IsSelected == true);
    public BasicList<D> ListSelectedDice()
    {
        if (ShowHold == true)
        {
            throw new CustomBasicException("The dice is being held, not selected");
        }
        return DiceList.GetSelectedItems();
    }
    public async Task<BasicList<BasicList<D>>> GetDiceList(string body)
    {
        return await js1.DeserializeObjectAsync<BasicList<BasicList<D>>>(body);
    }
    public BasicList<BasicList<D>> RollDice(int howManySections = 6)
    {
        if (DiceList.Count != HowManyDice)
        {
            RedoList();
        }
        BasicList<BasicList<D>> output = new();
        AsyncDelayer.SetDelayer(this, ref _delay!);
        IGenerateDice<int> thisG = MainContainer!.Resolve<IGenerateDice<int>>();
        BasicList<int> possList = thisG.GetPossibleList;
        D tempDice;
        int chosen;
        howManySections.Times(() =>
        {
            BasicList<D> firsts = new();
            for (int i = 0; i < HowManyDice; i++)
            {
                tempDice = DiceList[i];
                if (tempDice.Hold == false)
                {
                    chosen = possList.GetRandomItem();
                    tempDice = new()
                    {
                        Index = i + 1
                    };
                    tempDice.Populate(chosen);
                }
                firsts.Add(tempDice);
            }
            output.Add(firsts);
        });
        return output;
    }
    public void ClearDice()
    {
        HasDice = true;
        HowManyDice = _originalNumber;
        DiceList.Clear(HowManyDice);
    }
    private void RedoList()
    {
        DiceList.Clear(HowManyDice);
    }
    public async Task SendMessageAsync(BasicList<BasicList<D>> thisList)
    {
        await _network!.SendAllAsync("rolled", thisList);
    }
    public async Task SendMessageAsync(string category, BasicList<BasicList<D>> thisList)
    {
        await _network!.SendAllAsync(category, thisList);
    }
    public Action? UpdateDiceAction { get; set; }
    public async Task ShowRollingAsync(BasicList<BasicList<D>> diceCollection, bool showVisible)
    {
        CanShowDice = showVisible;
        AsyncDelayer.SetDelayer(this, ref _delay!);
        await diceCollection.ForEachAsync(async firsts =>
        {
            DiceList.ReplaceDiceRange(firsts);
            int tempCount = DiceList.Count;
            if (DiceList.Any(Items => Items.Index > tempCount || Items.Index <= 0))
            {
                throw new CustomBasicException("Index cannot be higher than the dicecount or less than 1");
            }
            HasDice = true;
            if (CanShowDice == true)
            {
                Visible = true;
                RefreshDice();
                await _delay.DelayMilli(50);
            }
        });
    }
    public void RefreshDice()
    {
        if (UpdateDiceAction == null)
        {
            CommandContainer.UpdateSpecificAction(CommandActionString);
        }
        else
        {
            UpdateDiceAction.Invoke(); //to accomodate trouble game or other games like it.
        }
    }
    public async Task ShowRollingAsync(BasicList<BasicList<D>> thisCol)
    {
        await ShowRollingAsync(thisCol, true);
    }
    public void SortDice(bool descending)
    {
        DiceList.SortDice(descending);
        RefreshDice();
    }
    public void ReplaceDiceRange(IBasicList<D> thisList)
    {
        DiceList.ReplaceDiceRange(thisList);
        HowManyDice = DiceList.Count;
    }
    public void ReplaceSelectedDice()
    {
        BasicList<D> TempList = DiceList.GetSelectedItems();
        DiceList.ReplaceDiceRange(TempList);
        HowManyDice = DiceList.Count;
    }
    public int ValueOfOnlyDice => DiceList.Single().Value;
    public IGamePackageGeneratorDI? GeneratorContainer { get; set; }
    public void RemoveSelectedDice()
    {
        if (ShowHold == true)
        {
            throw new CustomBasicException("Cannot remove selected dice because its being held instead");
        }
        DiceList.RemoveSelectedDice();
        HowManyDice = DiceList.Count;
    }
    public void RemoveConditionalDice(Predicate<D> predicate)
    {
        DiceList.RemoveConditionalDice(predicate);
        HowManyDice = DiceList.Count;
    }
    public void HideDice()
    {
        HasDice = false;
    }
    protected override void EnableChange()
    {
        DiceCommand.ReportCanExecuteChange();
    }
    protected override void PrivateEnableAlways() { }
}