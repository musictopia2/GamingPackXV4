namespace MonopolyCardGame.Core.Logic;
public class TradePile : HandObservable<MonopolyCardGameCardInformation>
{
    private bool _didProcess;
    public int GetPlayerIndex { get; }
    private readonly int _myID;
    private readonly MonopolyCardGameGameContainer _gameContainer;
    private readonly MonopolyCardGameVMData _model;
    private readonly IToast _toast;
    public TradePile(MonopolyCardGameGameContainer gameContainer, 
        MonopolyCardGameVMData model, 
        int player,
        IToast toast
        ) : base(gameContainer.Command)
    {
        _gameContainer = gameContainer;
        _model = model;
        GetPlayerIndex = player;
        _toast = toast;
        AutoSelect = EnumHandAutoType.None;
        _myID = _gameContainer.PlayerList!.GetSelf().Id;
        Text = "Trade " + player;
        Visible = true;
    }
    public bool IsCardSelected(int deck)
    {
        var thisCard = HandList.GetSpecificItem(deck);
        return thisCard.IsSelected;
    }
    public void SelectPastPoint(int deck)
    {
        bool starts = false;
        HandList.ForEach(thisCard =>
        {
            if (thisCard.Deck == deck)
            {
                starts = true;
            }
            thisCard.IsSelected = starts;
        });
    }
    public void SelectSpecificNumberOfCards(int howMany)
    {
        int x;
        int y = 0;
        for (x = HandList.Count; x >= 1; x += -1)
        {
            HandList[x - 1].IsSelected = true;
            y += 1;
            if (y == howMany)
            {
                break;
            }
        }
    }
    public void ClearBoard(MonopolyCardGameCardInformation thisCard)
    {
        HandList.ReplaceAllWithGivenItem(thisCard);
    }
    public void RemoveCards()
    {
        HandList.Clear();
    }
    public void AddCard(int deck, bool isSelected = false)
    {
        MonopolyCardGameCardInformation thisCard;
        thisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
        thisCard.IsSelected = isSelected;
        HandList.Add(thisCard);
    }
    public int HowManyCardsSelected() => HandList.Count(items => items.IsSelected);
    public DeckRegularDict<MonopolyCardGameCardInformation> GetSelectedItems => HandList.GetSelectedItems();
    public void GetNumberOfItems(int howMany)
    {
        howMany.Times(x =>
        {
            var thisCard = HandList.Last();
            thisCard.Drew = true;
            HandList.RemoveSpecificItem(thisCard);
            _gameContainer.SingleInfo!.MainHandList.Add(thisCard);
        });
        if (_gameContainer.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
        {
            _gameContainer.SingleInfo.MainHandList.Sort();
        }
    }
    public async Task PutBackAsync()
    {
        if (_gameContainer.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self && _gameContainer.BasicData.MultiPlayer)
        {
            await _gameContainer.Network!.SendAllAsync("putback");
        }
        var card = HandList.Last();
        HandList.RemoveSpecificItem(card);
        _gameContainer.SingleInfo!.MainHandList.Add(card);
        if (_gameContainer.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
        {
            _gameContainer.SingleInfo.MainHandList.Sort();
        }
        if (_previousStatus == EnumWhatStatus.None)
        {
            throw new CustomBasicException("The previous status cannot be none");
        }
        _gameContainer.SaveRoot.GameStatus = _previousStatus;
        _previousStatus = EnumWhatStatus.None;
    }
    private EnumWhatStatus _previousStatus;
    private async Task ProcessSelfPileAsync()
    {
        int numselected;
        int numunselected;
        numselected = _model.PlayerHand1!.HowManySelectedObjects;
        numselected += _model.TempSets1.HowManySelectedObjects;
        numunselected = _model.PlayerHand1.HowManyUnselectedObjects;
        numunselected += _model.TempSets1.HowManyUnselectedObjects;
        if (_gameContainer.SaveRoot.GameStatus == EnumWhatStatus.EndTurn)
        {
            _toast.ShowUserErrorToast("You can only end turn now");
            return;
        }
        if (numselected == 0)
        {
            _toast.ShowUserErrorToast("Sorry, you must select a card from your had to put to the trade pile");
            return;
        }
        if (_gameContainer.SaveRoot!.GameStatus == EnumWhatStatus.DrawOrTrade && numselected != 1)
        {
            _toast.ShowUserErrorToast("Sorry, only one card can be put from your hand to the trade pile because noone traded with you in the last turn");
            return;
        }
        if (_gameContainer.SaveRoot.GameStatus == EnumWhatStatus.TradeOnly && _gameContainer.SingleInfo!.ObjectCount < 10)
        {
            _toast.ShowUserErrorToast("Sorry, you already selected a card.  Therefore, you have to choose who to trade with");
            return;
        }
        if (_gameContainer.SaveRoot.GameStatus == EnumWhatStatus.Either && numunselected < 9)
        {
            _toast.ShowUserErrorToast("Sorry, you must leave at least 9 cards in your hand");
            return;
        }
        if (_gameContainer.SaveRoot.GameStatus == EnumWhatStatus.Discard && numunselected < 10)
        {
            _toast.ShowUserErrorToast("Sorry, you cannot discard cards to make your hand equal to less than 10");
            return;
        }
        TradePile thisTrade;
        thisTrade = this;
        if (_gameContainer.SaveRoot.GameStatus == EnumWhatStatus.Discard)
        {
            thisTrade.EndTurn();
        }
        DeckRegularDict<MonopolyCardGameCardInformation> thisCol = _model.PlayerHand1.ListSelectedObjects(true);
        thisCol.AddRange(_model.TempSets1.ListSelectedObjects(true));
        if (_gameContainer.BasicData!.MultiPlayer)
        {
            var newCol = thisCol.GetDeckListFromObjectList();
            await _gameContainer.Network!.SendAllAsync("trade1", newCol);
        }
        foreach (var thisCard in thisCol)
        {
            thisTrade.AddCard(thisCard.Deck, true);
        }
        if (_gameContainer.SaveRoot.GameStatus == EnumWhatStatus.Discard && _gameContainer.SingleInfo!.ObjectCount == 10)
        {
            await _gameContainer.EndTurnAsync!.Invoke();
            return;
        }
        SetPreviousStatus();
    }
    public void SetPreviousStatus()
    {
        if (_gameContainer.SaveRoot.GameStatus == EnumWhatStatus.Discard)
        {
            _previousStatus = EnumWhatStatus.None;
            return;
        }
        if (_gameContainer.SaveRoot.GameStatus == EnumWhatStatus.DrawOrTrade)
        {
            _previousStatus = _gameContainer.SaveRoot.GameStatus;
            _gameContainer.SaveRoot.GameStatus = EnumWhatStatus.TradeOnly;
            return;
        }
        _previousStatus = _gameContainer.SaveRoot.GameStatus;
        if (_gameContainer.SingleInfo!.MainHandList.Count == 9)
        {
            _gameContainer.SaveRoot.GameStatus = EnumWhatStatus.TradeOnly;
        }
        if (_gameContainer.SaveRoot.GameStatus == EnumWhatStatus.Either && _gameContainer.SingleInfo.ObjectCount < 10)
        {
            _gameContainer.SaveRoot.GameStatus = EnumWhatStatus.TradeOnly;
        }
    }
    protected override async Task ProcessObjectClickedAsync(MonopolyCardGameCardInformation thisObject, int index)
    {
        if (_myID == GetPlayerIndex)
        {
            await base.ProcessObjectClickedAsync(thisObject, index);
            return;
        }
        await ProcessOtherPilesAsync();
    }
    private async Task ProcessOtherPilesAsync()
    {
        MonopolyCardGamePlayerItem tempPlayer;
        tempPlayer = _gameContainer!.PlayerList![GetPlayerIndex];
        await Task.Delay(0);
        _gameContainer.StartCustomTrade?.Invoke(tempPlayer);
        return;
    }
    private async Task PlayerClickedAsync()
    {
        if (GetPlayerIndex == _myID)
        {
            await ProcessSelfPileAsync();
            return;
        }
        await ProcessOtherPilesAsync();
    }
    protected override async Task PrivateBoardSingleClickedAsync()
    {
        if (_didProcess)
        {
            _didProcess = false;
            return;
        }
        await PlayerClickedAsync();
    }
}