namespace BasicGameFrameworkLibrary.Core.BasicDrawables.BasicClasses;
/// <summary>
/// 
/// </summary>
/// <param name="privateDict"></param>
/// <param name="beforePopulate">This is extra stuff that has to happen before populating item if any</param>
public class BasicObjectShuffler<D>(IDeckDict<D> privateDict, Action<D>? beforePopulate = null) : IDeckShuffler<D>, IAdvancedDIContainer, ISerializable where D : IDeckObject, new()
{
    private IDeckCount? _deckCount; //maybe needed. to stop overflow exceptions.
    private readonly IRandomGenerator _rs = RandomHelpers.GetRandomGenerator();
    public IGamePackageResolver? MainContainer { get; set; }
    public IGamePackageGeneratorDI? GeneratorContainer { get; set; }
    public bool NeedsToRedo { get; set; }
    public void RelinkObject(int oldDeck, D newObject)
    {
        privateDict.RemoveObjectByDeck(oldDeck);
        privateDict.Add(newObject);
    }
    public void UnlinkObjects()
    {
        privateDict.ForEach(items => items.Reset());
    }
    private void EnableObjects()
    {
        privateDict.ForEach(items => items.IsEnabled = true);
    }
    private void FixObjects()
    {
        EnableObjects();
        privateDict.ForEach(items =>
        {
            items.Drew = false;
            items.IsSelected = false;
            items.IsUnknown = false;
            items.Visible = true;
            items.IsEnabled = true;
            items.Reset();
        });
    }
    private bool RedoList()
    {
        if (privateDict.Count == 0)
        {
            return true;
        }
        return NeedsToRedo;
    }

    private INewCard<D>? _newCard;
    public void ClearObjects()
    {
        privateDict.Clear();
    }
    public int GetDeckCount()
    {
        if (_deckCount == null)
        {
            PopulateContainer(this);
            _deckCount = MainContainer!.Resolve<IDeckCount>(); //i think
            if (MainContainer.RegistrationExist<INewCard<D>>())
            {
                _newCard = MainContainer.Resolve<INewCard<D>>();
            }
        }
        return _deckCount.GetDeckCount();
    }
    public async Task<DeckRegularDict<D>> GetListFromJsonAsync(string jsonData)
    {
        bool doRedo = RedoList();
        BasicList<int> thisList = await js1.DeserializeObjectAsync<BasicList<int>>(jsonData);
        BasicList<D> tempList = new();
        if (doRedo == true)
        {
            thisList.ForEach(Items =>
            {
                D thisD = new();
                beforePopulate!.Invoke(thisD); //sometimes something has to be done before the card can be populated.
                thisD.Populate(Items);
                tempList.Add(thisD);
            });
            privateDict.AddRange(tempList);
            return new(tempList);
        }
        if (privateDict.Count == thisList.Count)
        {
            FixObjects();
            DeckRegularDict<D> newTemp = new(privateDict);
            thisList.ForEach(Items =>
            {
                tempList.Add(newTemp.GetSpecificItem(Items));
            });
            privateDict.ReplaceRange(tempList);
            return new DeckRegularDict<D>(tempList);
        }
        UnlinkObjects();
        thisList.ForEach(items =>
        {
            D thisD = privateDict.GetSpecificItem(items);
            thisD.Visible = true;
            thisD.IsEnabled = true;
            thisD.IsUnknown = false;
            thisD.IsSelected = false;
            thisD.Drew = false;
            tempList.Add(thisD);
        });
        return new DeckRegularDict<D>(tempList);
    }
    public D GetSpecificItem(int deck)
    {
        return privateDict.GetSpecificItem(deck);
    }
    public void OrderedObjects()
    {
        PrivatePopulate();
    }
    private D GetItem(int chosen)
    {
        if (_newCard == null)
        {
            return new D();
        }
        return _newCard.GetNewCard(chosen); //async should not be required here. this interface is useful for games like fluxx.
    }
    private void PrivatePopulate()
    {
        int maxs = GetDeckCount();
        for (int i = 1; i <= maxs; i++)
        {
            D thisD = GetItem(i);
            beforePopulate?.Invoke(thisD);
            thisD.Populate(i);
            privateDict.Add(thisD);
        }
    }
    public void ShuffleObjects()
    {
        bool redo = RedoList();
        if (redo == false)
        {
            FixObjects();
            privateDict.ShuffleList();
            return;
        }
        PrivatePopulate();
        privateDict.ShuffleList();
    }
    public void ReshuffleFirstObjects(IDeckDict<D> thisList, int startAt, int endAt)
    {
        int x = 0;
        int index;
        int ask1;
        endAt = endAt - thisList.Count + 1;
        int increasedEnd = 0;
        thisList.ForEach(items =>
        {
            index = privateDict.IndexOf(items);
            if (index <= endAt)
            {
                increasedEnd++;
            }
        });
        endAt += increasedEnd;
        thisList.ForEach(items =>
        {
            index = privateDict.IndexOf(items);
            if (index == -1)
            {
                throw new CustomBasicException("Item not found to reshuffle the card");
            }
            if (index < startAt || index > endAt)
            {
                ask1 = _rs!.GetRandomNumber(endAt, startAt);
                privateDict.MoveItem(items, ask1);
            }
            x++;
            endAt--;
        });
    }
    public bool ObjectExist(int deck)
    {
        return privateDict.ObjectExist(deck);
    }
    public void PutCardOnTop(int deck)
    {
        var card = privateDict.GetSpecificItem(deck);
        privateDict.RemoveObjectByDeck(deck);
        privateDict.InsertBeginning(card);
    }
}