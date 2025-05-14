namespace DealCardGame.Core.Logic;
public class PersonalCompleteSets
{
    public PersonalCompleteSets(DealCardGameGameContainer gameContainer,
        IToast toast,
        PrivateAutoResumeProcesses privateAutoResume
        )
    {
        TempHand = new(gameContainer.Command);
        TempHand.AutoSelect = EnumHandAutoType.None; //has to be none because has to run code to decide what to do.
        TempHand.Text = "Temporary Hand";
        TempHand.ObjectClickedAsync = SelectFromHandAsync;
        _gameContainer = gameContainer;
        _toast = toast;
        _privateAutoResume = privateAutoResume;
        CreateHand();
    }
    private Task SelectFromHandAsync(DealCardGameCardInformation card, int index)
    {
        if (_cardChosen is not null)
        {
            _cardChosen.IsSelected = false; //has to select the new one instead.
            _cardChosen = null; //something else is chosen.
        }
        if (card.IsSelected)
        {
            card.IsSelected = false;
        }
        else
        {
            _gameContainer.PersonalInformation.TemporaryCards.ForEach(card => card.IsSelected = false);
            card.IsSelected = true;
        }
        return Task.CompletedTask;
    }
    public Func<EnumColor, Task>? SetClickedAsync { get; set; }
    public HandObservable<DealCardGameCardInformation> TempHand { get; set; }
    public BasicList<PropertySetHand> SetList { get; set; } = [];
    private DealCardGameCardInformation? _cardChosen;
    private readonly DealCardGameGameContainer _gameContainer;
    private readonly IToast _toast;
    private readonly PrivateAutoResumeProcesses _privateAutoResume;
    private void CreateHand()
    {
        PropertySetHand set;
        10.Times(x =>
        {
            set = new(_gameContainer.Command);
            EnumColor color = EnumColor.FromValue(x);
            set.Text = EnumColor.FromValue(x).ToString(); //so you know which color this represents.
            if (color == EnumColor.MediumVioletRed)
            {
                set.Text = "Light Purple";
            }
            set.AutoSelect = EnumHandAutoType.None;
            set.ObjectClickedAsync = SelectObjectAsync;
            set.SetClickedAsync = ThisSet_SetClickedAsync; //cannot set maximums.  otherwise, causes too much unnecessary scrolling
            SetList.Add(set); //i think.
        });
    }
    public bool HasSelectedCard => _cardChosen is not null;
    private Task SelectObjectAsync(DealCardGameCardInformation card, int index)
    {
        if (_cardChosen is not null)
        {
            card.IsSelected = false;
            _cardChosen = null;
            return Task.CompletedTask; //to select/unselect card.
        }
        _cardChosen = card;
        _cardChosen.IsSelected = true;
        //must choose one at a time for this.  since you are mostly organizing the wilds.
        return Task.CompletedTask;
    }
    private bool CanAddCard(DealCardGameCardInformation card, EnumColor color)
    {
        var property = _gameContainer.PersonalInformation.SetData.Single(x => x.Color == color);
        if (card.ActionCategory == EnumActionCategory.House || card.ActionCategory == EnumActionCategory.Hotel)
        {
            if (property!.HasRequiredSet() == false)
            {
                _toast.ShowUserErrorToast("Unable to add card because don't have a monopoly to even add a house or hotel");
                return false;
            }
            if (card.ActionCategory == EnumActionCategory.Hotel)
            {
                if (property.HasRequiredHouse() == false)
                {
                    _toast.ShowUserErrorToast("Unable to add card because you need a house first");
                    return false;
                }
            }
            return true;
        }
        if (property!.HasRequiredSet())
        {
            _toast.ShowUserErrorToast("You already have a monopoly");
            return false;
        }
        if (card.AnyColor)
        {
            return true;
        }
        if (card.FirstColorChoice == property!.Color || card.SecondColorChoice == property.Color)
        {
            return true;
        }
        _toast.ShowUserErrorToast("Wrong color for monopoly to place card");
        return false;
    }
    private bool CanRemoveCard(DealCardGameCardInformation card)
    {
        if (card.ActionCategory == EnumActionCategory.Hotel)
        {
            return true;
        }
        var property = _gameContainer.PersonalInformation.SetData.GetPropertyFromCard(card.Deck);
        if (card.ActionCategory == EnumActionCategory.House)
        {
            if (property!.HasRequiredHotel())
            {
                _toast.ShowUserErrorToast("You must remove the hotel before you can remove the house");
                return false;
            }
        }
        if (property!.HasRequiredSet())
        {
            if (property!.Cards.Any(x => x.ActionCategory == EnumActionCategory.House || x.ActionCategory == EnumActionCategory.Hotel))
            {
                _toast.ShowUserErrorToast("Must remove the house and hotel before removing card because you break up the set");
                return false;
            }
        }
        return true;
    }
    public bool IsAcceptable()
    {
        //this assumes you used up all required cards.
        bool rets = TempHand.HandList.Count == 0;
        if (rets == false)
        {
            _toast.ShowUserErrorToast("You must use all your cards");
            return false;
        }
        var list = _gameContainer.PersonalInformation.SetData;
        foreach ( var property in list )
        {
            if (property.Cards.Any(x => x.ActionCategory == EnumActionCategory.House))
            {
                if (property.HasRequiredSet() == false)
                {
                    _toast.ShowUserErrorToast("You no longer have the required set for a house");
                    return false;
                }
            }
        }
        return true;
    }
    public void Init()
    {
        TempHand.HandList.ReplaceRange(_gameContainer.PersonalInformation.TemporaryCards);
        PopulateSets();
    }
    private void PopulateSets()
    {
        foreach (var item in _gameContainer.PersonalInformation.SetData)
        {
            int index = _gameContainer.PersonalInformation.SetData.IndexOf(item);
            PropertySetHand hand = SetList[index];
            hand.DidClickObject = false;
            hand.HandList.ReplaceRange(item.Cards); //i think.
        }
    }
    public async Task ResetAsync(bool doSave)
    {
        TempHand.ClearHand();
        var player = _gameContainer.PlayerList!.GetSelf();
        player.ClonePlayerProperties(_gameContainer.PersonalInformation);
        PopulateSets();
        if (doSave)
        {
            await _privateAutoResume.SaveStateAsync(_gameContainer); //i think.
        }
    }
    public async Task AddCardToSetAsync(EnumColor color)
    {
        DealCardGameCardInformation card;
        bool fromHand = false;
        if (_cardChosen is not null)
        {
            card = _cardChosen;
            _cardChosen = null;
            if (card is null)
            {
                throw new CustomBasicException("Cant' be null anymore");
            }
        }    
        else
        {
            fromHand = true;
            if (TempHand.HasSelectedObject() == false)
            {
                _toast.ShowUserErrorToast("Did No card was selected anywhere");
                return;
            }
            card = TempHand.ListSelectedObjects(true).Single(); //should be just one card selected.
            _gameContainer.PersonalInformation.TemporaryCards.RemoveObjectByDeck(card.Deck); //just in case.
        }
        card.IsSelected = false;
        if (CanAddCard(card, color) == false)
        {
            if (fromHand)
            {
                TempHand.HandList.Add(card);
                _gameContainer.PersonalInformation.TemporaryCards.Add(card); //because it can't be added.  otherwise, goes to waste.
            }
            return; //cannot even add card.  should unselect the card as well.
        }
        SetPropertiesModel property;
        if (fromHand == false)
        {
            //needs to remove from the set.
            if (CanRemoveCard(card) == false)
            {
                return;
            }
            property = _gameContainer.PersonalInformation.SetData.GetPropertyFromCard(card.Deck)!;
            _gameContainer.PersonalInformation.SetData.RemoveCardFromPlayerSet(card.Deck, property!.Color);
            SetList[property.Color.Value - 1].HandList.RemoveSpecificItem(card);
        }
        property = _gameContainer.PersonalInformation.SetData.Single(x => x.Color == color);
        property.Cards.Add(card);
        SetList[property.Color.Value - 1].HandList.Add(card);
        SetList.ForEach(set => set.DidClickObject = false);
        await _privateAutoResume.SaveStateAsync(_gameContainer);
    }
    public async Task AddCardToTemporaryHandAsync()
    {
        if (_cardChosen is null)
        {
            _toast.ShowUserErrorToast("No card was still chosen");
            return;
        }
        var property = _gameContainer.PersonalInformation.SetData.GetPropertyFromCard(_cardChosen!.Deck);
        EnumColor color = property!.Color;
        _cardChosen.IsSelected = false;
        property.Cards.RemoveObjectByDeck(_cardChosen.Deck);
        int index = color.Value - 1; //because 0 based.
        SetList[index].HandList.RemoveObjectByDeck(_cardChosen.Deck);
        SetList[index].DidClickObject = false;
        TempHand.HandList.Add(_cardChosen);
        _gameContainer.PersonalInformation.TemporaryCards.Add(_cardChosen);
        _cardChosen = null; //not anymore.
        await _privateAutoResume.SaveStateAsync(_gameContainer);
    }
    private async Task ThisSet_SetClickedAsync(PropertySetHand set)
    {
        if (SetClickedAsync == null)
        {
            return;
        }
        if (set.DidClickObject)
        {
            set.DidClickObject = false;
            return;
        }
        int index = SetList.IndexOf(set) + 1;
        await SetClickedAsync.Invoke(EnumColor.FromValue(index));
    }
}