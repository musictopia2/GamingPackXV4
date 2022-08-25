namespace PyramidSolitaire.Core.Logic;
[SingletonGame]
public class PyramidSolitaireMainGameClass : RegularDeckOfCardsGameClass<SolitaireCard>, IAggregatorContainer
{
    private readonly ISaveSinglePlayerClass _thisState;
    private readonly IToast _toast;
    private readonly ISystemError _error;
    internal PyramidSolitaireSaveInfo SaveRoot { get; set; }
    internal bool GameGoing { get; set; }
    public PyramidSolitaireMainGameClass(ISaveSinglePlayerClass thisState,
        IEventAggregator aggregator,
        IToast toast,
        ISystemError error,
        IGamePackageResolver container
        )
    {
        _thisState = thisState;
        Aggregator = aggregator;
        _toast = toast;
        _error = error;
        SaveRoot = container.ReplaceObject<PyramidSolitaireSaveInfo>(); //can't create new one.  because if doing that, then anything that needs it won't have it.
        SaveRoot.Load(Aggregator);
    }
    private PyramidSolitaireMainViewModel? _model;
    public Task NewGameAsync(PyramidSolitaireMainViewModel model)
    {
        GameGoing = true;
        _model = model;
        return base.NewGameAsync(model.DeckPile);
    }
    public override Task NewGameAsync(DeckObservablePile<SolitaireCard> deck)
    {
        throw new CustomBasicException("Wrong");
    }
    public IEventAggregator Aggregator { get; }
    public override async Task<bool> CanOpenSavedSinglePlayerGameAsync()
    {
        return await _thisState.CanOpenSavedSinglePlayerGameAsync();
    }
    public override Task OpenSavedGameAsync()
    {
        throw new CustomBasicException("Unable to open save game.  Rethink");
    }
    private bool _isBusy;
    public async Task SaveStateAsync()
    {
        if (_isBusy)
        {
            return;
        }
        _isBusy = true;
        SaveRoot.DeckList = DeckPile!.GetCardIntegers();
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
    protected override void AfterShuffle()
    {
        _model!.CurrentPile!.ClearCards();
        SaveRoot.Score = 0;
        _model.Discard!.ClearCards();
        var newList = _model.DeckPile!.DrawSeveralCards(28);
        _model.PlayList1!.ObjectList.Clear();
        _model.GameBoard1!.ClearCards(newList);
    }
    public async Task DrawCardAsync()
    {
        if (HasPlayedCard())
        {
            _toast.ShowUserErrorToast("Sorry, you must choose to either play the cards or put them back before drawing another card");
            return;
        }
        bool wasEnd = DeckPile!.IsEndOfDeck();
        if (wasEnd)
        {
            var thisCol = _model!.Discard!.DiscardList();
            if (_model.Discard.PileEmpty() == false)
            {
                thisCol.Add(_model.Discard.GetCardInfo());
            }
            if (_model.CurrentPile!.PileEmpty() == false)
            {
                thisCol.Add(_model.CurrentPile.GetCardInfo());
            }
            _model.CurrentPile.ClearCards();
            _model.Discard.ClearCards();
            _model.DeckPile.OriginalList(thisCol);
        }
        var thisCard = _model!.DeckPile.DrawCard();
        if (_model.CurrentPile!.PileEmpty())
        {
            _model.CurrentPile.AddCard(thisCard);
        }
        else
        {
            var newCard = _model.CurrentPile.GetCardInfo();
            _model.Discard!.AddCard(newCard);
            _model.CurrentPile.AddCard(thisCard);
        }
        SaveRoot.RecentCard = _model.CurrentPile.GetCardInfo();
        if (wasEnd)
        {
            await Aggregator.PublishAsync(new PossibleNewGameEventModel());
        }
    }
    public bool HasPlayedCard() => _model!.PlayList1!.HasChosenCards();
    public bool IsValidMove()
    {
        var thisCol = _model!.PlayList1!.ObjectList;
        int maxs = thisCol.Sum(items => items.Value.Value);
        return maxs == 13;
    }
    public void PutBack()
    {
        var thisCol = _model!.PlayList1!.ObjectList;
        thisCol.ForEach(thisCard =>
        {
            if (_model.GameBoard1!.CanPutBack(thisCard.Deck) == false)
            {
                if (thisCard.Deck == SaveRoot.RecentCard.Deck)
                {
                    _model.CurrentPile!.AddCard(thisCard);
                }
                else
                {
                    _model.Discard!.AddCard(thisCard);
                }
            }
        });
        _model.GameBoard1!.PutBackAll();
        _model.PlayList1.RemoveCards();
    }
    public void PutBack(SolitaireCard card)
    {
        _model!.PlayList1.RemoveOneCard(card);
        if (_model!.GameBoard1.CanPutBack(card.Deck) == false)
        {
            if (card.Deck == SaveRoot.RecentCard.Deck)
            {
                _model.CurrentPile.AddCard(card);
            }
            else
            {
                _model.Discard.AddCard(card);
            }
        }
        else
        {
            _model.GameBoard1.PutBackOne(card.Deck);
        }
    }
    public bool CanAddToPlay() => !_model!.PlayList1!.AlreadyHasTwoCards();
    public async Task PlayCardsAsync()
    {
        var thisCol = _model!.PlayList1!.ObjectList;
        SaveRoot.Score += thisCol.Count;
        if (_model.CurrentPile!.PileEmpty() == false)
        {
            SaveRoot.RecentCard = _model.CurrentPile.GetCardInfo();
        }
        _model.PlayList1.RemoveCards();
        _model.GameBoard1!.MakePermanant();
        if (SaveRoot.Score == 52)
        {
            await ShowWinAsync();
            return;
        }
    }
}