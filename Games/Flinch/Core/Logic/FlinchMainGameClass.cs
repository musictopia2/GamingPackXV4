namespace Flinch.Core.Logic;
[SingletonGame]
public class FlinchMainGameClass
    : CardGameClass<FlinchCardInformation, FlinchPlayerItem, FlinchSaveInfo>
    , IMiscDataNM, ISerializable
{
    private readonly FlinchVMData _model;
    private readonly CommandContainer _command;
    private readonly FlinchGameContainer _gameContainer;
    private readonly FlinchComputerAI _ai;
    private readonly IToast _toast;
    internal BasicList<ComputerData> ComputerList { get; set; } = new();
    public FlinchMainGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        FlinchVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<FlinchCardInformation> cardInfo,
        CommandContainer command,
        FlinchGameContainer gameContainer,
        FlinchComputerAI ai,
        ISystemError error,
        IToast toast
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, error, toast)
    {
        _model = currentMod;
        _command = command;
        _gameContainer = gameContainer;
        _ai = ai;
        _toast = toast;
        _model.PublicPiles.PileClickedAsync = PublicPiles_PileClickedAsync;
        _gameContainer.IsValidMove = IsValidMove;
    }
    private async Task PublicPiles_PileClickedAsync(int index)
    {
        if (SaveRoot!.GameStatus == EnumStatusList.DiscardAll)
        {
            _toast.ShowUserErrorToast("Sorry; you must discard all your cards");
            return;
        }
        int decks = CardSelected(out EnumCardType types, out int discardNum);
        if (decks == 0 && types == EnumCardType.IsNone)
        {
            _toast.ShowUserErrorToast("Sorry, there was nothing selected");
            return;
        }
        if (decks == 0)
        {
            throw new CustomBasicException("Nothing selected but the type was not none");
        }
        bool rets = IsValidMove(index, decks);
        if (rets == false)
        {
            _toast.ShowUserErrorToast("Illegal Move");
            return;
        }
        await PlayOnPileAsync(index, decks, types, discardNum);
    }
    public override Task FinishGetSavedAsync()
    {
        SaveRoot!.LoadMod(_model!);
        LoadControls();
        _model.PublicPiles!.PileList!.ReplaceRange(SaveRoot.PublicPileList);
        return base.FinishGetSavedAsync();
    }
    protected override Task MiddleReshuffleCardsAsync(IDeckDict<FlinchCardInformation> thisList, bool canSend)
    {
        if (thisList.Count != SaveRoot!.CardsToShuffle)
        {
            throw new CustomBasicException($"Must have {SaveRoot!.CardsToShuffle}, not {thisList.Count}");
        }
        var nextList = _model!.Pile1!.DiscardList();
        if (nextList.Count > 0)
        {
            throw new CustomBasicException("The discard list somehow did not get cleared out");
        }
        return base.MiddleReshuffleCardsAsync(thisList, canSend);
    }
    protected override void PrepStartTurn()
    {
        base.PrepStartTurn();
        ComputerList = new();
    }
    private void LoadControls()
    {
        if (IsLoaded == true)
        {
            return;
        }
        IsLoaded = true;
    }
    private async Task ComputerDiscardAsync()
    {
        FlinchComputerAI.ComputerDiscardInfo thisDiscard;
        thisDiscard = _ai!.ComputerDiscard();
        await AddToDiscardAsync(thisDiscard.Pile, thisDiscard.Deck);
    }
    protected override async Task ComputerTurnAsync()
    {
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelayMilli(250);
        }
        _ai!.MaxPiles = _model.PublicPiles!.MaxPiles();
        if (ComputerList.Count == 0)
        {
            ComputerList = _ai!.ComputerMoves();
            if (ComputerList.Count == 0)
            {
                if (SaveRoot.GameStatus == EnumStatusList.FirstOne)
                {
                    if (BasicData!.MultiPlayer == true)
                    {
                        await Network!.SendEndTurnAsync();
                    }
                    await EndTurnAsync();
                    return;
                }
                await ComputerDiscardAsync();
                return;
            }
        }
        ComputerData thisItem = ComputerList.First();
        if (thisItem.WhichType == EnumCardType.IsNone)
        {
            throw new CustomBasicException("Needs to find a card type to play one");
        }
        await PlayOnPileAsync(thisItem.Pile, thisItem.CardToPlay!.Deck, thisItem.WhichType, thisItem.Discard);
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        LoadControls();
        ComputerList.Clear();
        return base.StartSetUpAsync(isBeginning);
    }
    protected override Task LastPartOfSetUpBeforeBindingsAsync()
    {
        SaveRoot!.CardsToShuffle = 0;
        _model.PublicPiles!.ClearBoard();
        SaveRoot.GameStatus = EnumStatusList.FirstOne;
        SaveRoot.LoadMod(_model!);
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.DiscardList.Clear();
            thisPlayer.StockList.ReplaceRange(thisPlayer.MainHandList);
            thisPlayer.MainHandList.Clear();
            thisPlayer.StockLeft = thisPlayer.StockList.Count;
            thisPlayer.InStock = thisPlayer.StockList.Last().Display;
            thisPlayer.Discard1 = "0";
            thisPlayer.Discard2 = "0";
            thisPlayer.Discard3 = "0";
            thisPlayer.Discard4 = "0";
            thisPlayer.Discard5 = "0";
        });
        return base.LastPartOfSetUpBeforeBindingsAsync();
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "discardnew":
                SendDiscard discard = await js1.DeserializeObjectAsync<SendDiscard>(content);
                await AddToDiscardAsync(discard.Pile, discard.Deck);
                return;
            case "play":
                SendPlay play = await js1.DeserializeObjectAsync<SendPlay>(content);
                await PlayOnPileAsync(play.Pile, play.Deck, play.WhichType, play.Discard);
                return;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override async Task StartNewTurnAsync()
    {
        await base.StartNewTurnAsync();
        if (_gameContainer.LoadPlayerPilesAsync != null)
        {
            await _gameContainer.LoadPlayerPilesAsync.Invoke(); //i think.
        }
        await StartDrawingAsync();
    }
    public override async Task EndTurnAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        SingleInfo.MainHandList.UnhighlightObjects();
        if (Test!.ComputerEndsTurn == false && SingleInfo.PlayerCategory == EnumPlayerCategory.Computer)
        {
            if (SingleInfo.MainHandList.Count > 4 && (SaveRoot!.GameStatus == EnumStatusList.DiscardOneOnly || SaveRoot.GameStatus == EnumStatusList.Normal))
            {
                throw new CustomBasicException("Cannot have 5 cards left at the end of this turn");
            }
            else if (SingleInfo.MainHandList.Count > 0 && SaveRoot!.GameStatus == EnumStatusList.DiscardAll)
            {
                throw new CustomBasicException("Must discard all the cards based on the game status");
            }
        }
        SingleInfo.StockList = _model.StockPile!.GetStockList();
        SingleInfo.DiscardList = _model.DiscardPiles!.PileList!.ToBasicList();
        _command.ManuelFinish = true; //because it could be somebody else's turn.
        WhoTurn = await PlayerList.CalculateWhoTurnAsync();
        if (SaveRoot!.PlayerFound > 0 && SingleInfo.MainHandList.Count == 5)
        {
            SaveRoot.GameStatus = EnumStatusList.DiscardOneOnly;
        }
        else if ((SaveRoot.PlayerFound == 0) & (SingleInfo.MainHandList.Count == 5))
        {
            SaveRoot.GameStatus = EnumStatusList.DiscardAll;
        }
        else if ((SingleInfo.MainHandList.Count < 5) & (SaveRoot.PlayerFound > 0))
        {
            SaveRoot.GameStatus = EnumStatusList.Normal;
        }
        this.ShowTurn();
        await SaveStateAsync();
        await StartNewTurnAsync();
    }
    public async override Task PopulateSaveRootAsync()
    {
        SaveRoot!.PublicPileList = _model.PublicPiles.PileList!.ToBasicList();
        await base.PopulateSaveRootAsync();
    }
    public override async Task ContinueTurnAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        if (SingleInfo.MainHandList.Count == 0)
        {
            await SaveStateAsync();
            await StartDrawingAsync();
        }
        else
        {
            await base.ContinueTurnAsync();
        }
    }
    private bool _wasNew;
    private async Task StartDrawingAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        if (SingleInfo.MainHandList.Count == 5)
        {
            await ContinueTurnAsync();
            return; // because you already have 5 cards now.
        }
        if (SingleInfo.MainHandList.Count == 0)
        {
            _wasNew = true;
        }
        else
        {
            _wasNew = false;
        }
        LeftToDraw = 5 - SingleInfo.MainHandList.Count;
        PlayerDraws = WhoTurn;
        if (LeftToDraw == 1)
        {
            LeftToDraw = 0;
            PlayerDraws = 0;
        }
        await DrawAsync();
    }
    public void UnselectAllCards()
    {
        _model.StockPile!.UnselectCard();
        _model.PublicPiles!.UnselectAllPiles();
        _model!.PlayerHand1!.UnselectAllObjects();
        _model.DiscardPiles!.UnselectAllCards();
    }
    private void UpdateDiscardData(FlinchPlayerItem thisPlayer, int thisPile)
    {
        string thisNum;
        if (_model.DiscardPiles!.HasCard(thisPile) == false)
        {
            thisNum = "0";
        }
        else
        {
            var card = _model.DiscardPiles.GetLastCard(thisPile);
            thisNum = card.Display; // i think needs to be value.  So can even be W
        }
        switch (thisPile)
        {
            case 0:
                {
                    thisPlayer.Discard1 = thisNum;
                    break;
                }

            case 1:
                {
                    thisPlayer.Discard2 = thisNum;
                    break;
                }

            case 2:
                {
                    thisPlayer.Discard3 = thisNum;
                    break;
                }

            case 3:
                {
                    thisPlayer.Discard4 = thisNum;
                    break;
                }
            case 4:
                {
                    thisPlayer.Discard5 = thisNum;
                    break; //to support flinch.
                }
            default:
                {
                    throw new CustomBasicException("Piles are only 0 to 3, not " + thisPile + ".  Remember, 0 based now");
                }
        }
    }
    private bool HasOne()
    {
        if (_model.StockPile.NextCardInStock() == "1")
        {
            return true;
        }
        return SingleInfo!.MainHandList.Any(items => items.Number == 1);
    }
    private void RemoveFromHand(int deck)
    {
        SingleInfo!.MainHandList.RemoveObjectByDeck(deck);
        if (SingleInfo.MainHandList.Count == 5)
        {
            throw new CustomBasicException($"After removing a card, must have less than 5 cards left for {SingleInfo.NickName}");
        }
        _command.UpdateAll();
    }
    protected override async Task AfterDrawingAsync()
    {
        if (_wasNew == true)
        {
            SingleInfo!.MainHandList.UnhighlightObjects();
        }
        if (SingleInfo!.MainHandList.Any(items => items.IsUnknown))
        {
            throw new CustomBasicException("Should never have unknown cards");
        }
        if (HasOne() == true)
        {
            await AutomatePlayOneAsync();
        }
        await base.AfterDrawingAsync();
    }
    private async Task AutomatePlayOneAsync()
    {
        FlinchCardInformation thisCard;
        thisCard = SingleInfo!.MainHandList.FirstOrDefault(x => x.Number == 1)!;
        EnumCardType thisCat;
        if (thisCard == null)
        {
            thisCard = _model.StockPile.GetCard();
            thisCat = EnumCardType.Stock;
        }
        else
        {
            thisCat = EnumCardType.MyCards;
        }
        await PlayOnPileAsync(-1, thisCard.Deck, thisCat, -1);
    }
    protected override Task AfterReshuffleAsync()
    {
        SaveRoot!.CardsToShuffle = 0;
        return base.AfterReshuffleAsync();
    }
    private void RemoveFromPile(int pile)
    {
        var thisCol = _model.PublicPiles!.EmptyPileList(pile);
        thisCol.ForEach(thisCard =>
        {
            SaveRoot!.CardsToShuffle++;
            _model!.Pile1!.AddCard(thisCard);
        });
    }
    public async Task AddToDiscardAsync(int pile, int deck)
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        if (SingleInfo.CanSendMessage(BasicData!) == true)
        {
            SendDiscard thisDiscard = new();
            thisDiscard.Deck = deck;
            thisDiscard.Pile = pile;
            await Network!.SendAllAsync("discardnew", thisDiscard);
        }
        if (SingleInfo.MainHandList.Count > 5)
        {
            throw new CustomBasicException($"The hand must be 5 or less, not {SingleInfo.MainHandList.Count} for {SingleInfo.NickName}");
        }
        RemoveFromHand(deck);
        var thisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
        if (thisCard.IsUnknown)
        {
            throw new CustomBasicException("Can not be unknown when playing on discard pile.  Rethink");
        }
        thisCard.Drew = false;
        thisCard.IsSelected = false;
        await AnimateDiscardAsync(thisCard, pile, EnumAnimcationDirection.StartUpToCard);
        _model.DiscardPiles!.AddCardToPile(pile, thisCard);
        UpdateDiscardData(SingleInfo, pile);
        _command.UpdateAll();
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelaySeconds(.5);
        }
        if (SaveRoot!.GameStatus == EnumStatusList.DiscardAll && SingleInfo.MainHandList.Count > 0)
        {
            await ContinueTurnAsync();
            return;
        }
        if (SaveRoot.GameStatus == EnumStatusList.DiscardAll && SingleInfo.MainHandList.Count == 0)
        {
            SaveRoot.GameStatus = EnumStatusList.FirstOne;
            await ContinueTurnAsync(); //since it will automatically draw 5 cards now
            return;
        }
        await EndTurnAsync();
    }
    public async Task AnimateDiscardAsync(FlinchCardInformation thisCard, int pile, EnumAnimcationDirection direction)
    {
        await Aggregator!.AnimateCardAsync(thisCard, direction, $"discard{SingleInfo!.NickName}", _model.DiscardPiles!.PileList![pile]);
    }
    public async Task AnimateStockAsync(FlinchCardInformation thisCard)
    {
        var thisPile = _model.StockPile!.StockFrame.PileList!.Single();
        await Aggregator!.AnimateCardAsync(thisCard, EnumAnimcationDirection.StartCardToUp, $"stock{SingleInfo!.NickName}", thisPile);
    }
    public bool IsValidMove(int pile, int deck)
    {
        if (pile == -1)
        {
            throw new CustomBasicException("The pile cannot be -1 for a valid move");
        }
        var thisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
        int nexts = _model.PublicPiles!.NextNumberNeeded(pile);
        return nexts == thisCard.Number;
    }
    public int CardSelected(out EnumCardType whatType, out int discardNum)
    {
        int thisNum = _model!.PlayerHand1!.ObjectSelected();
        if (thisNum > 0)
        {
            whatType = EnumCardType.MyCards;
            discardNum = -1;
            return thisNum;
        }
        thisNum = _model!.DiscardPiles!.CardSelected(out int thisPile);
        if (thisNum > 0)
        {
            whatType = EnumCardType.Discard;
            discardNum = thisPile;
            if (discardNum == -1)
            {
                throw new CustomBasicException("Must be something from the discard pile");
            }
            return thisNum;
        }
        discardNum = -1;
        thisNum = _model.StockPile!.CardSelected();
        if (thisNum > 0)
        {
            whatType = EnumCardType.Stock;
            return thisNum;
        }
        whatType = EnumCardType.IsNone;
        return 0;
    }
    public async Task PlayOnPileAsync(int pile, int deck, EnumCardType whichType, int discardNum)
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        if (whichType == EnumCardType.IsNone)
        {
            throw new CustomBasicException("Must have a card type in order to play on a pile");
        }
        if (pile == -1 && SaveRoot!.PlayerFound == 0)
        {
            SaveRoot.PlayerFound = WhoTurn;
        }
        if (SingleInfo.CanSendMessage(BasicData!) == true && pile > -1)
        {
            SendPlay thisPlay = new();
            thisPlay.Deck = deck;
            thisPlay.Pile = pile;
            thisPlay.WhichType = whichType;
            thisPlay.Discard = discardNum;
            await Network!.SendAllAsync("play", thisPlay);
        }
        var thisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
        if (thisCard.IsUnknown)
        {
            throw new CustomBasicException("Should not have been unknown.  Rethink");
        }
        thisCard.Drew = false;
        thisCard.IsSelected = false;
        if (whichType == EnumCardType.MyCards)
        {
            RemoveFromHand(deck);
        }
        else if (whichType == EnumCardType.Stock)
        {
            _model.StockPile!.RemoveCard();
            _ = SingleInfo.StockList.RemoveObjectByDeck(thisCard.Deck);
            await AnimateStockAsync(thisCard);
            SingleInfo.StockLeft = _model.StockPile.CardsLeft();
            SingleInfo.InStock = _model.StockPile.NextCardInStock();
        }
        else
        {
            if (discardNum == -1)
            {
                throw new CustomBasicException("The discard number cannot be -1");
            }
            if (_model.DiscardPiles!.PileList![discardNum].ObjectList.Count == 0)
            {
                throw new CustomBasicException("The discard must have at least one item to play from");
            }
            _model.DiscardPiles!.RemoveCard(discardNum, deck);
            UpdateDiscardData(SingleInfo, discardNum);
            await AnimateDiscardAsync(thisCard, discardNum, EnumAnimcationDirection.StartCardToUp);
            _model.DiscardPiles!.UnselectAllCards();
        }
        _command.UpdateAll();
        if (pile > -1)
        {
            var npile = _model.PublicPiles!.PileList[pile];
            await Aggregator.AnimateCardAsync(thisCard, EnumAnimcationDirection.StartDownToCard, "public", npile);
            _model.PublicPiles.AddCardToPile(pile, thisCard);
            _command.UpdateAll();
            if (_model.PublicPiles.NeedToRemovePile(pile))
            {
                if (Test!.NoAnimations == false)
                {
                    await Delay!.DelaySeconds(1);
                }
                RemoveFromPile(pile);
            }
        }
        else
        {
            _model.PublicPiles!.CreateNewPile(thisCard);
        }
        if (whichType == EnumCardType.Stock)
        {
            if (_model.StockPile.DidGoOut() == true)
            {
                await ShowWinAsync();
                return;
            }
        }
        if (SingleInfo.PlayerCategory == EnumPlayerCategory.Computer && pile > -1)
        {
            if (BasicData!.MultiPlayer == false || BasicData.Client == false)
            {
                ComputerList.RemoveFirstItem();
            }
        }
        if (HasOne() == true)
        {
            await AutomatePlayOneAsync();
            return; //because you have to play if you do have a one.
        }
        await ContinueTurnAsync(); //since this will check first to see if it has to draw 5 more cards.
    }
}