﻿namespace BasicGameFrameworkLibrary.Core.DrawableListsObservable;

public partial class HandObservable<D> : SimpleControlObservable where D : IDeckObject, new()
{
    public bool Visible { get; set; } = true;
    public ControlCommand? ObjectSingleClickCommand { get; set; } //one parameter.
    public ControlCommand? BoardSingleClickCommand { get; set; }
    public HandObservable(CommandContainer command) : base(command)
    {
        CreateCommands();
    }
    partial void CreateCommands();
    public EnumHandAutoType AutoSelect { get; set; } = EnumHandAutoType.SelectOneOnly;
    public bool HasFrame { get; set; } = true;
    protected virtual bool CanEverEnable()
    {
        return true; // usually, it can.   however sometimes, you can't
    }
    public string Text { get; set; } = "";
    public int Maximum { get; set; } //useful for ui  the cross platform can set this
    public int SectionClicked { get; set; }
    public virtual bool HasSections => false;
    public bool IgnoreMaxRules { get; set; }

    public DeckRegularDict<D> HandList = new();
    private readonly DeckRegularDict<D> _orderOfObjectsSelectedList = new();
    public DeckRegularDict<D> GetObjectsInOrderSelected(bool alsoRemove)
    {
        if (alsoRemove == true)
        {
            _ = ListSelectedObjects(true);
            var newList = _orderOfObjectsSelectedList.ToRegularDeckDict();
            _orderOfObjectsSelectedList.Clear();
            return newList;
        }
        return _orderOfObjectsSelectedList.ToRegularDeckDict();
    }
    public void PopulateSavedCards(BasicList<int> list)
    {
        DeckRegularDict<D> temp = new();
        foreach (var deck in list)
        {
            D card = new();
            card.Populate(deck);
            temp.Add(card);
        }
        PopulateObjects(temp);
    }
    public BasicList<int> GetSavedCards()
    {
        if (HandList.Count == 0)
        {
            return new();
        }
        return HandList.GetDeckListFromObjectList();
    }
    public void PopulateObjects(IDeckDict<D> thisList) // try just t (if regularcardinfo; then that will be the list.  knows it has to be at least baseimages.cardinfo
    {
        if (IgnoreMaxRules == false)
        {
            if (thisList.Count > Maximum && Maximum > 0)
            {
                throw new CustomBasicException("The maximum objects allowed are " + Maximum);
            }
        }
        HandList.ReplaceRange(thisList); // i think its being replace.  if its different, can fix
        AfterPopulateObjects();
    }
    protected virtual void AfterPopulateObjects() { }
    public void ClearHand()
    {
        HandList.Clear();
        _orderOfObjectsSelectedList.Clear();
    }
    public virtual void EndTurn()
    {
        HandList.UnhighlightObjects();
    }
    public void SelectAllObjects()
    {
        HandList.SelectAllObjects();
        _orderOfObjectsSelectedList.Clear(); // because its doing automatically instead of manually
    }
    public virtual void UnselectAllObjects() //decided to make it virtual so overrided versions can do other things after unselecting all or before.
    {
        HandList.UnselectAllObjects();
        _orderOfObjectsSelectedList.Clear();
    }
    public bool HasSelectedObject() => HandList.Any(items => items.IsSelected);
    public int ObjectSelected()
    {
        try
        {
            var id = (from x in HandList
                      where x.IsSelected == true
                      select x.Deck).SingleOrDefault();
            return id; // try this way.
        }
        catch (Exception)
        {
            throw new CustomBasicException("There was more than one object selected");
        }
    }
    public DeckRegularDict<D> ListSelectedObjects()
    {
        return ListSelectedObjects(false);
    }
    public DeckRegularDict<D> ListSelectedObjects(bool alsoRemove)
    {
        DeckRegularDict<D> newList = HandList.Where(x => x.IsSelected == true).ToRegularDeckDict();
        if (alsoRemove == true)
        {
            HandList.RemoveSelectedItems();
        }
        return newList;
    }
    public int HowManySelectedObjects => HandList.Count(x => x.IsSelected == true);
    public int HowManyUnselectedObjects => HandList.Count(x => x.IsSelected == false);
    public void SelectOneFromDeck(int deck)
    {
        D thisObject;
        thisObject = HandList.GetSpecificItem(deck);
        thisObject.IsSelected = true;
    }
    public void UnselectOneFromDeck(int deck)
    {
        D thisObject;
        thisObject = HandList.GetSpecificItem(deck);
        thisObject.IsSelected = false;
    }
    [Command(EnumCommandCategory.Control, Name = nameof(BoardSingleClickCommand))]
    protected virtual async Task PrivateBoardSingleClickedAsync()
    {
        if (BoardClickedAsync == null)
        {
            return;
        }
        await BoardClickedAsync.Invoke(); //most likely an issue because using async.
    }
    protected virtual bool CanSelectSingleObject(D thisObject)
    {
        return true; // so something else can do something else
    }
    protected override void SetCommandsLimited()
    {
        ObjectSingleClickCommand!.BusyCategory = EnumCommandBusyCategory.Limited;
        BoardSingleClickCommand!.BusyCategory = EnumCommandBusyCategory.Limited;
    }
    [Command(EnumCommandCategory.Control, Name = nameof(ObjectSingleClickCommand))]
    public async Task ObjectClickProcessAsync(D thisObject)
    {
        if (AutoSelect == EnumHandAutoType.ShowObjectOnly)
        {
            if (ConsiderSelectOneAsync != null)
            {
                await ConsiderSelectOneAsync.Invoke(thisObject);
            }
            if (BoardClickedAsync is not null)
            {
                await BoardClickedAsync.Invoke();
                return; //try this way.  there are cases where a card is show only but clicking has to do board click.
            }
        }
        if (AutoSelect == EnumHandAutoType.SelectAsMany)
        {
            if (ConsiderSelectOneAsync != null)
            {
                await ConsiderSelectOneAsync.Invoke(thisObject);
            }
            if (thisObject.IsSelected == true)
            {
                if (_orderOfObjectsSelectedList.ObjectExist(thisObject.Deck) == true)
                {
                    _orderOfObjectsSelectedList.RemoveSpecificItem(thisObject);
                }
                thisObject.IsSelected = false;
                if (AutoSelectedOneCompletedAsync != null)
                {
                    await AutoSelectedOneCompletedAsync.Invoke();
                }
                return;
            }
            if (BeforeAutoSelectObjectAsync != null)
            {
                await BeforeAutoSelectObjectAsync.Invoke();
            }
            thisObject.IsSelected = true;
            if (_orderOfObjectsSelectedList.ObjectExist(thisObject.Deck) == false)
            {
                _orderOfObjectsSelectedList.Add(thisObject);
            }
            if (AutoSelectedOneCompletedAsync != null)
            {
                await AutoSelectedOneCompletedAsync.Invoke();
            }
            return;
        }
        if (AutoSelect == EnumHandAutoType.SelectOneOnly)
        {
            if (CanSelectSingleObject(thisObject) == false)
            {
                return;
            }
            await ProcessSelectOneOnlyAsync(thisObject);

            return;
        }
        await ProcessObjectClickedAsync(thisObject, HandList.IndexOf(thisObject));
    }
    //added new overrided method here so games like savannah, you can do something special for selectoneonly.
    protected virtual async Task ProcessSelectOneOnlyAsync(D payLoad)
    {
        if (ConsiderSelectOneAsync != null)
        {
            await ConsiderSelectOneAsync.Invoke(payLoad);
        }
        if (payLoad.IsSelected == true)
        {
            payLoad.IsSelected = false;
            if (AutoSelectedOneCompletedAsync != null)
            {
                await AutoSelectedOneCompletedAsync.Invoke();
            }
            return;
        }
        HandList.UnselectAllObjects();
        if (BeforeAutoSelectObjectAsync != null)
        {
            await BeforeAutoSelectObjectAsync.Invoke();
        }
        payLoad.IsSelected = true;
        if (AutoSelectedOneCompletedAsync != null)
        {
            await AutoSelectedOneCompletedAsync.Invoke();
        }
    }
    protected virtual async Task ProcessObjectClickedAsync(D thisObject, int index)
    {
        if (ObjectClickedAsync == null)
        {
            return;
        }
        await ObjectClickedAsync.Invoke(thisObject, HandList.IndexOf(thisObject));
    }

    #region Events
    protected async Task OnBoardClickedAsync()
    {
        if (BoardClickedAsync == null)
        {
            return;
        }
        await BoardClickedAsync.Invoke();
    }
    protected override void EnableChange() { }
    protected override bool CanEnableFirst()
    {
        return CanEverEnable();
    }
    protected override void PrivateEnableAlways()
    {
        BoardSingleClickCommand!.BusyCategory = EnumCommandBusyCategory.Limited;
        ObjectSingleClickCommand!.BusyCategory = EnumCommandBusyCategory.Limited;
    }

    public event ObjectClickedEventHandler? ObjectClickedAsync;
    public delegate Task ObjectClickedEventHandler(D payLoad, int index);
    public event AutoSelectedOneCompletedEventHandler? AutoSelectedOneCompletedAsync;
    public delegate Task AutoSelectedOneCompletedEventHandler();
    public event BeforeAutoSelectCardEventHandler? BeforeAutoSelectObjectAsync;
    public delegate Task BeforeAutoSelectCardEventHandler();
    public event BoardClickedEventHandler? BoardClickedAsync;
    public delegate Task BoardClickedEventHandler();
    public event ConsiderSelectOneEventHandler? ConsiderSelectOneAsync; // this is needed for games like fluxx.  so when you click on one, it can show the details of the card you chose.  since there is no mouse over.
    public delegate Task ConsiderSelectOneEventHandler(D payLoad);
    #endregion
}