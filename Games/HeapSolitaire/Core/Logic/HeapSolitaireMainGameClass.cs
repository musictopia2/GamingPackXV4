namespace HeapSolitaire.Core.Logic;
[SingletonGame]
public class HeapSolitaireMainGameClass : RegularDeckOfCardsGameClass<HeapSolitaireCardInfo>, IAggregatorContainer
{
    private readonly ISaveSinglePlayerClass _thisState;
    private readonly IRandomGenerator _rs;
    private readonly IToast _toast;
    private readonly ISystemError _error;
    internal HeapSolitaireSaveInfo SaveRoot { get; set; }
    internal bool GameGoing { get; set; }
    public HeapSolitaireMainGameClass(ISaveSinglePlayerClass thisState,
        IEventAggregator aggregator,
        IGamePackageResolver container,
        IRandomGenerator rs,
        IToast toast,
        ISystemError error
        )
    {
        _thisState = thisState;
        Aggregator = aggregator;
        _rs = rs;
        _toast = toast;
        _error = error;
        SaveRoot = container.ReplaceObject<HeapSolitaireSaveInfo>(); //can't create new one.  because if doing that, then anything that needs it won't have it.
        SaveRoot.Load(aggregator);
    }
    private HeapSolitaireMainViewModel? _model;
    public Task NewGameAsync(HeapSolitaireMainViewModel model)
    {
        GameGoing = true;
        _model = model;
        return base.NewGameAsync(model.DeckPile);
    }
    public override Task NewGameAsync(DeckObservablePile<HeapSolitaireCardInfo> deck)
    {
        throw new CustomBasicException("Please use other process");
    }
    public IEventAggregator Aggregator { get; }
    public override async Task<bool> CanOpenSavedSinglePlayerGameAsync()
    {
        return await _thisState.CanOpenSavedSinglePlayerGameAsync();
    }
    public override async Task OpenSavedGameAsync()
    {
        DeckList.OrderedObjects(); //i think
        SaveRoot = await _thisState.RetrieveSinglePlayerGameAsync<HeapSolitaireSaveInfo>();
        if (SaveRoot.DeckList.Count > 0)
        {
            var newList = SaveRoot.DeckList.GetNewObjectListFromDeckList(DeckList);
            DeckPile!.OriginalList(newList);
        }
        _model!.Main1.PileList!.ReplaceRange(SaveRoot.MainPiles!);
        _model.Main1.RefreshInfo();
        SaveRoot.Load(Aggregator);
        _model.Waste1!.PileList!.ReplaceRange(SaveRoot.WasteData!);
    }
    private bool _isBusy;
    public async Task SaveStateAsync()
    {
        if (_isBusy)
            return;
        _isBusy = true;
        SaveRoot.DeckList = DeckPile!.GetCardIntegers();
        SaveRoot.MainPiles = _model!.Main1.PileList!.ToBasicList();
        SaveRoot.WasteData = _model.Waste1.PileList!.ToBasicList();
        await _thisState.SaveSimpleSinglePlayerGameAsync(SaveRoot); //i think
        _isBusy = false;
    }
    public async Task ShowWinAsync()
    {
        _toast.ShowSuccessToast("Congratulations, you won");
        await Task.Delay(2000);
        GameGoing = false;
        await this.SendGameOverAsync(_error);
    }
    private DeckRegularDict<HeapSolitaireCardInfo> _cardList = new();
    private DeckRegularDict<HeapSolitaireCardInfo> GetFirstList()
    {
        DeckRegularDict<HeapSolitaireCardInfo> output = new();
        EnumRegularColorList newColor = EnumRegularColorList.FromValue(_rs.GetRandomNumber(2));
        int newNumber = 7;
        do
        {
            if (output.Count < 13)
            {
                if (newColor == EnumRegularColorList.Black)
                {
                    newColor = EnumRegularColorList.Red;
                }
                else
                {
                    newColor = EnumRegularColorList.Black;
                }
                var finalCard = _cardList.First(items => items.Value.Value == newNumber && items.Color == newColor);
                output.Add(finalCard);
                _cardList.RemoveSpecificItem(finalCard);
                if (output.Count == 13)
                {
                    return output;
                }
                newNumber++;
                if (newNumber > 13)
                {
                    newNumber = 1;
                }
            }
        } while (true);
    }
    protected override void AfterShuffle()
    {
        _cardList = DeckList.ToRegularDeckDict();
        _cardList.ForEach(thisCard => thisCard.IsUnknown = false);
        var firstCol = GetFirstList();
        SaveRoot.Score = 13;
        _model!.Main1.ClearBoard(firstCol);
        _model.Waste1!.ClearBoard(_cardList);
    }
    private bool IsValidMove(int whichOne, out HeapSolitaireCardInfo thisCard)
    {
        var prevCard = _model!.Main1.GetLastCard(whichOne);
        var newCard = _model.Waste1!.GetCard();
        thisCard = newCard;
        if (whichOne == 12 && prevCard.Color == newCard.Color)
        {
            return false;
        }
        else if (prevCard.Color != newCard.Color && whichOne != 12)
        {
            return false;
        }
        if (prevCard.Value == EnumRegularCardValueList.King && newCard.Value == EnumRegularCardValueList.LowAce)
        {
            return true;
        }
        if (prevCard.Value == EnumRegularCardValueList.King)
        {
            return false;
        }
        return prevCard.Value.Value + 1 == newCard.Value.Value;
    }
    public async Task SelectMainAsync(int whichOne)
    {
        if (_model!.Waste1.DidSelectCard == false)
        {
            _toast.ShowUserErrorToast("Sorry, you must select a card to put to the main one");
            return;
        }
        if (IsValidMove(whichOne, out HeapSolitaireCardInfo thisCard) == false)
        {
            _toast.ShowUserErrorToast("Illegal Move");
            return;
        }
        _model!.Main1.AddCardToPile(whichOne, thisCard);
        _model.Waste1.RemoveCardFromPile(SaveRoot.PreviousSelected);

        SaveRoot.PreviousSelected = -1;
        if (SaveRoot.Score == 104)
        {
            await ShowWinAsync();
        }
    }
}