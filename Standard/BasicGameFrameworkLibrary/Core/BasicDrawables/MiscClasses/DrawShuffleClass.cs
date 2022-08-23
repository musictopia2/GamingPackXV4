namespace BasicGameFrameworkLibrary.Core.BasicDrawables.MiscClasses;

/// <summary>
/// This class is used for games that is not technically a card game but has cards like Candyland, or Sorry Board Game.
/// Has routines for drawing, shuffling and reshuffling.
/// </summary>
public class DrawShuffleClass<D, P> : nm.IDrawCardNM, nm.IReshuffledCardsNM
    where D : class, IDeckObject, new() where P : class, IPlayerItem, new()
{
    public ISavedCardList<D>? SaveRoot; //this is all it needs.
    private readonly IListShuffler<D> _deckList;
    private readonly IGameNetwork? _network;
    private readonly BasicData _basicData;
    private readonly IToast _toast;

    public Func<P>? CurrentPlayer { get; set; }
    public Func<Task>? AfterDrawingAsync { get; set; }
    public Action<IListShuffler<D>>? AfterFirstShuffle { get; set; }
    public Action<IListShuffler<D>>? RemovePossibleReshuffledCards { get; set; }
    public async Task DrawAsync()
    {
        if (CurrentPlayer == null)
        {
            throw new CustomBasicException("Current player function was never filled out.  Rethink");
        }
        if (SaveRoot!.CardList!.Count == 0)
        {
            if (_isBeginning == true)
            {
                throw new CustomBasicException("Should not already be reshuffling because its the beginning of the game");
            }
            bool canSendMessage;
            canSendMessage = CurrentPlayer.Invoke().CanSendMessage(_basicData);
            if (canSendMessage == true || _basicData.MultiPlayer == false)
            {
                await ReshuffleCardsAsync(canSendMessage);
            }
            else
            {
                _network!.IsEnabled = true;
            }
            return;
        }
        SaveRoot.CurrentCard = SaveRoot.CardList.GetFirstObject(true);
        if (_isBeginning == false)
        {
            if (AfterDrawingAsync == null)
            {
                throw new CustomBasicException("The after drawing was never populated.  Rethink");
            }
            await AfterDrawingAsync.Invoke();
        }
        else
        {
            _isBeginning = false;
        }
    }
    private async Task ReshuffleCardsAsync(bool canSend)
    {
        _deckList.ClearObjects();
        _deckList.ShuffleObjects();
        if (RemovePossibleReshuffledCards is not null)
        {
            RemovePossibleReshuffledCards.Invoke(_deckList); //to accomodate a game like risk board game
        }
        if (canSend == true)
        {
            BasicList<int> newList = _deckList.ExtractIntegers(xx => xx.Deck);
            await _network!.SendAllAsync("reshuffledcards", newList);
        }
        SaveRoot!.CardList = _deckList.ToRegularDeckDict();
        await AfterReshuffleAsync();
    }
    private async Task AfterReshuffleAsync()
    {
        _toast.ShowInfoToast("Its the end of the deck; therefore; the cards are being reshuffled");
        await DrawAsync();
    }
    private bool _isBeginning;
    public async Task FirstShuffleAsync(bool canAutoDraw)
    {
        _deckList.ClearObjects(); //just in case.
        _deckList.ShuffleObjects();
        if (AfterFirstShuffle != null)
        {
            AfterFirstShuffle.Invoke(_deckList);
        }
        SaveRoot!.CardList = _deckList.ToRegularDeckDict();
        if (canAutoDraw == true)
        {
            _isBeginning = true;
            await DrawAsync();
        }
        else
        {
            SaveRoot.CurrentCard = new D(); //not sure if we need this (but could).
        }
    }
    public DrawShuffleClass(IListShuffler<D> deckList, BasicData basicData, IToast toast)
    {
        _deckList = deckList;
        if (basicData.MultiPlayer)
        {
            _network = Resolver!.Resolve<IGameNetwork>();
        }
        _basicData = basicData;
        _toast = toast;
    }
    async Task nm.IDrawCardNM.DrawCardReceivedAsync(string data)
    {
        await DrawAsync();
    }
    async Task nm.IReshuffledCardsNM.ReshuffledCardsReceived(string data)
    {
        BasicList<int> firstList = await js.DeserializeObjectAsync<BasicList<int>>(data);
        if (_deckList.Any() == false)
        {
            _deckList.OrderedObjects(); //maybe this was needed.  i think this is the best way to handle this situation.
        }
        DeckRegularDict<D> newList = firstList.GetNewObjectListFromDeckList(_deckList);
        SaveRoot!.CardList = newList.ToRegularDeckDict();
        await AfterReshuffleAsync();
    }
}