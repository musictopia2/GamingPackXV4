namespace TriangleSolitaire.Core.Logic;
[SingletonGame]
public class TriangleSolitaireMainGameClass : RegularDeckOfCardsGameClass<SolitaireCard>, IAggregatorContainer
{
    private readonly ISaveSinglePlayerClass _thisState;
    private readonly IGamePackageResolver _container;
    private readonly IToast _toast;
    private readonly ISystemError _error;
    internal TriangleSolitaireSaveInfo _saveRoot;
    private EnumIncreaseList Incs
    {
        set => _saveRoot.Incs = value;
        get => _saveRoot.Incs;
    }
    internal bool GameGoing { get; set; }
    public TriangleSolitaireMainGameClass(ISaveSinglePlayerClass thisState,
        IEventAggregator aggregator,
        IGamePackageResolver container,
        IToast toast,
        ISystemError error
        )
    {
        _thisState = thisState;
        Aggregator = aggregator;
        _container = container;
        _toast = toast;
        _error = error;
        _saveRoot = container.ReplaceObject<TriangleSolitaireSaveInfo>(); //can't create new one.  because if doing that, then anything that needs it won't have it.
    }
    public override Task NewGameAsync(DeckObservablePile<SolitaireCard> deck)
    {
        GameGoing = true;
        return base.NewGameAsync(deck);
    }
    public IEventAggregator Aggregator { get; }
    public override async Task<bool> CanOpenSavedSinglePlayerGameAsync()
    {
        return await _thisState.CanOpenSavedSinglePlayerGameAsync();
    }
    public override async Task OpenSavedGameAsync()
    {
        DeckList.OrderedObjects(); //i think
        _saveRoot = await _thisState.RetrieveSinglePlayerGameAsync<TriangleSolitaireSaveInfo>();
        if (_saveRoot.DeckList.Count > 0)
        {
            var newList = _saveRoot.DeckList.GetNewObjectListFromDeckList(DeckList);
            DeckPile!.OriginalList(newList);
        }
    }
    private bool _isBusy;
    public async Task SaveStateAsync()
    {
        if (_isBusy)
        {
            return;
        }
        _isBusy = true;
        _saveRoot.DeckList = DeckPile!.GetCardIntegers();
        await _thisState.SaveSimpleSinglePlayerGameAsync(_saveRoot); //i think
        _isBusy = false;
    }
    public async Task ShowWinAsync()
    {
        _toast.ShowSuccessToast("Congratulations, you won");
        await Task.Delay(2000);
        GameGoing = false;
        await this.SendGameOverAsync(_error);
    }
    internal Action? InitDraw { get; set; }
    protected override void AfterShuffle()
    {
        Incs = EnumIncreaseList.None;
        InitDraw?.Invoke();
    }
    public void DrawCard(TriangleSolitaireMainViewModel model)
    {
        model.Pile1.AddCard(DeckPile!.DrawCard());
        Incs = EnumIncreaseList.None;
    }
    public bool IsValidMove(SolitaireCard controlCard, SolitaireCard pileCard, out EnumIncreaseList tempi)
    {
        tempi = EnumIncreaseList.None;
        if (controlCard.Color == pileCard.Color)
        {
            return false; //has to be opposite colors.
        }
        if (controlCard.Value == pileCard.Value)
        {
            return false; //can't be the same number either.
        }
        bool ispileAce = false;
        if (pileCard.Value == EnumRegularCardValueList.LowAce || pileCard.Value == EnumRegularCardValueList.HighAce)
        {
            ispileAce = true;
            tempi = Incs;
            if (Incs == EnumIncreaseList.Increase)
            {
                return controlCard.Value == EnumRegularCardValueList.King;
            }
            if (Incs == EnumIncreaseList.Decrease)
            {
                return controlCard.Value == EnumRegularCardValueList.Two;
            }
        }
        if (controlCard.Value == EnumRegularCardValueList.King && ispileAce)
        {
            Incs = EnumIncreaseList.Increase;
            return true;
        }
        if (controlCard.Value == EnumRegularCardValueList.Two && ispileAce)
        {
            Incs = EnumIncreaseList.Decrease;
            return true;
        }
        bool isControlAce = controlCard.Value == EnumRegularCardValueList.LowAce || controlCard.Value == EnumRegularCardValueList.HighAce;
        if (isControlAce)
        {
            if (pileCard.Value == EnumRegularCardValueList.King)
            {
                Incs = EnumIncreaseList.Decrease;
                return true;
            }
            if (pileCard.Value == EnumRegularCardValueList.Two)
            {
                Incs = EnumIncreaseList.Increase;
                return true;
            }
            Incs = EnumIncreaseList.None;
            return false;
        }
        if (controlCard.Value.Value + 1 == pileCard.Value.Value)
        {
            Incs = EnumIncreaseList.Increase;
            return true;
        }
        Incs = EnumIncreaseList.Decrease;
        return controlCard.Value.Value - 1 == pileCard.Value.Value;
    }
    public async Task MakeMoveAsync(int whichOne, EnumIncreaseList tempi, TriangleSolitaireMainViewModel model)
    {
        Incs = tempi;
        var thisCard = model.Triangle1!.CardList.Single(items => items.Deck == whichOne);
        var pileCard = model.Pile1!.GetCardInfo();
        model.Triangle1.MakeInvisible(whichOne);
        var newCard = new SolitaireCard();
        newCard.Populate(thisCard.Deck);
        newCard.Visible = true;
        model.Pile1.AddCard(newCard);
        if (model.Triangle1.HowManyCardsLeft == 0)
        {
            await ShowWinAsync();
        }
    }
}