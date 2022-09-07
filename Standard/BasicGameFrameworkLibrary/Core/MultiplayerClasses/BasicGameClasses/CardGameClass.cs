using ps = BasicGameFrameworkLibrary.Core.BasicDrawables.MiscClasses; //not common enough.
namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.BasicGameClasses;
public abstract class CardGameClass<D, P, S> : BasicGameClass<P, S>, ICardGameMainProcesses<D>,
    IBeginningCards<D, P, S>
    , IReshuffledCardsNM, IDrawCardNM,
    IPickUpNM, IDiscardNM
    where D : class, IDeckObject, new()
    where P : class, IPlayerSingleHand<D>, new()
    where S : BasicSavedCardClass<P, D>, new()
{
    private readonly IBasicCardGamesData<D> _model;
    private readonly CardGameContainer<D, P, S> _gameContainer;
    private readonly IToast _toast;
    public CardGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        IBasicCardGamesData<D> currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<D> cardInfo,
        CommandContainer command,
        CardGameContainer<D, P, S> gameContainer,
        ISystemError error,
        IToast toast
        )
        : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, command, gameContainer, error, toast)
    {
        _model = currentMod;
        CardInfo = cardInfo;
        _gameContainer = gameContainer;
        _toast = toast;
        _gameContainer.DrawAsync = DrawAsync;
        _gameContainer.SortAfterDrawing = SortAfterDrawing;
        _gameContainer.SortCards = SortCards;
        _gameContainer.AnimatePlayAsync = AnimatePlayAsync;
    }

    private bool _didReshuffle;
    public int PlayerDraws
    {
        get => _gameContainer.PlayerDraws;
        set => _gameContainer.PlayerDraws = value;
    }
    public int LeftToDraw
    {
        get => _gameContainer.LeftToDraw;
        set => _gameContainer.LeftToDraw = value;
    }
    public bool DoDraw { get; set; }
    public ICardInfo<D> CardInfo { get; }
    protected override async Task ShowHumanCanPlayAsync()
    {
        await base.ShowHumanCanPlayAsync();
        _model.PlayerHand1.ReportCanExecuteChange();
    }
    public override Task FinishGetSavedAsync()
    {
        _gameContainer.DeckList!.ClearObjects();
        _gameContainer.DeckList.OrderedObjects();
        var newList = SaveRoot!.PublicDeckList.GetNewObjectListFromDeckList(_gameContainer.DeckList);
        if (newList.Count > 0)
        {
            _model.Deck1!.OriginalList(newList);
        }
        _model.Pile1!.SavedDiscardPiles(SaveRoot.PublicDiscardData!);
        if (_gameContainer.GameInfo!.SinglePlayerChoice != EnumPlayerChoices.HumanOnly)
        {
            SingleInfo = PlayerList.GetSelf();
        }
        if (_model.PlayerHand1!.Visible == true)
        {
            if (CardInfo!.PlayerGetsCards == true)
            {
                SetHand();
                PrepSort(); 
                SortCards();
            }
        }
        if (_model.OtherPile != null)
        {
            if (SaveRoot.CurrentCard > 0)
            {
                var card = _gameContainer.DeckList.GetSpecificItem(SaveRoot.CurrentCard);
                if (_model.OtherPile.PileEmpty() == true)
                {
                    _model.OtherPile.AddCard(card);
                }
            }
            else
            {
                if (_model.OtherPile.PileEmpty() == false)
                {
                    _model.OtherPile.ClearCards();
                }
            }
        }
        return Task.CompletedTask;
    }
    public override Task StartNewTurnAsync()
    {
        PrepStartTurn();
        _gameContainer.AlreadyDrew = false;
        _gameContainer.PreviousCard = 0;
        PlayerDraws = 0;
        this.ShowTurn();
        return Task.CompletedTask;
    }
    /// <summary>
    /// this is used to get extra information needed.
    /// for example card games needs extras for deck and discard piles.
    /// had to do this way so when the host sends the game state, they will have the extra data to deserialize properly.
    /// it is suggested to do the original plus your extras.
    /// has to be public so it can be used from another class
    /// </summary>
    /// <returns></returns>
    public override Task PopulateSaveRootAsync()
    {
        if (_model.PlayerHand1!.Maximum > 0 && _model.PlayerHand1.IgnoreMaxRules == false)
        {
            if (SingleInfo!.MainHandList.Count > _model.PlayerHand1.Maximum) //needs to apply to both players.
            {
                throw new CustomBasicException("You have too many cards.  Rethink");
            }
        }
        SaveRoot!.PublicDiscardData = _model.Pile1!.GetSavedPile();
        SaveRoot.PublicDeckList = _model.Deck1!.GetCardIntegers();
        if (_model.OtherPile != null)
        {
            if (_model.OtherPile.PileEmpty())
            {
                SaveRoot.CurrentCard = 0;
            }
            else
            {
                SaveRoot.CurrentCard = _model.OtherPile.GetCardInfo().Deck;
            }
        }
        return Task.CompletedTask;
    }
    public virtual async Task EndRoundAsync() 
    {
        await SaveRoundAsync();
    }
    protected virtual Task SaveRoundAsync()
    {
        SaveRoot!.NewRound = true;
        return Task.CompletedTask;
    }
    protected bool PlayerCanWin()
    {
        if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Computer)
        {
            return true;
        }
        if (Test!.ComputerEndsTurn == true || Test.ComputerNoCards == true)
        {
            return false;
        }
        return true;
    }
    public async Task DiscardAsync(int deck)
    {
        var thisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
        thisCard.Drew = false;
        thisCard.IsSelected = false;
        await DiscardAsync(thisCard);
    }
    protected async Task AnimatePlayAsync(D thisCard)
    {
        if (_model.OtherPile != null && _model.OtherPile.CurrentOnly == true)
        {
            _model.OtherPile.ClearCards();
        }
        _gameContainer.Command.UpdateAll();
        if (Test!.NoAnimations == false)
        {
            await Aggregator.AnimatePlayAsync(thisCard, finalAction: () =>
            {
                if (_model.Pile1!.Visible == true)
                {
                    _model.Pile1.AddCard(thisCard);
                }
            });
        }
        else if (_model.Pile1!.Visible == true)
        {
            _model.Pile1.AddCard(thisCard);
        }
    }
    public virtual async Task DiscardAsync(D thisCard)
    {
        await AnimatePlayAsync(thisCard);
        await AfterDiscardingAsync();
    }
    protected virtual async Task AfterDiscardingAsync()
    {
        await EndTurnAsync();
    }
    protected virtual bool CanReshuffleAgain => true;
    protected virtual Task NoReshuffleAgainAsync()
    {
        return Task.CompletedTask;
    }
    public virtual async Task DrawAsync()
    {
        if (_model.Deck1.IsCutting)
        {
            return;
        }
        DoDraw = true;
        if (PlayerDraws == 0)
        {
            PlayerDraws = WhoTurn;
            if (LeftToDraw == 0)
            {
                LeftToDraw = 1;
            }
            SingleInfo = PlayerList!.GetWhoPlayer();
            _gameContainer.AlreadyDrew = true;
        }
        else if (CardInfo!.PlayerGetsCards == false)
        {
            LeftToDraw = 1;
        }
        else if (LeftToDraw == 0)
        {
            LeftToDraw = 1;
        }
        do
        {
            if (_model.Deck1!.IsEndOfDeck() == true && _model.Deck1.NeverAutoDisable == false)
            {
                await AfterDrawingAsync();
                return;
            }
            if (_model.Deck1.IsEndOfDeck() == true)
            {
                if (_didReshuffle == true)
                {
                    throw new CustomBasicException("Already reshuffled.  Therefore; must be a problem.  Find out what happened");
                }
                _didReshuffle = true;
                if (CanReshuffleAgain == false)
                {
                    await NoReshuffleAgainAsync();
                    return;
                }
                bool canSendMessage;
                canSendMessage = SingleInfo!.CanSendMessage(BasicData!); //try to use this function here too.
                if (canSendMessage == true || BasicData!.MultiPlayer == false)
                {
                    if (CardInfo!.ShowMessageWhenReshuffling == true)
                    {
                        _toast.ShowInfoToast("Its the end of the deck; therefore; the cards are being reshuffled");
                    }
                    await ReshuffleCardsAsync(canSendMessage);
                }
                else
                {
                    Network!.IsEnabled = true;
                }
                return;
            }
            _didReshuffle = false;
            LeftToDraw--;
            var thisCard = _model.Deck1.DrawCard();
            if (CardInfo!.HasDrawAnimation == true && Test!.NoAnimations == false)
            {
                await Aggregator.AnimateDrawAsync(thisCard);
            }
            if (CardInfo.PlayerGetsCards == false)
            {
                await PlayerReceivesNoCardsAfterDrawingAsync(thisCard);
                return;
            }
            if (_model.OtherPile != null && CardToCurrentPile() == true)
            {
                _model.OtherPile.AddCard(thisCard);
                await AfterDrawingAsync();
                return;
            }
            var tempPlayer = PlayerList![PlayerDraws]; //not 0 based anymore.
            if (ShowNewCardDrawn(tempPlayer) == true)
            {
                thisCard.Drew = true;
            }
            await AddCardAsync(thisCard, tempPlayer);
            if (LeftToDraw == 0)
            {
                if (tempPlayer.PlayerCategory == EnumPlayerCategory.Self)
                {
                    SortAfterDrawing();
                }
                await AfterDrawingAsync();
                return;
            }

        } while (true);
    }
    protected virtual void SortAfterDrawing() //games like blades of steele could do something else.
    {
        SortCards();
    }
    protected virtual Task AddCardAsync(D thisCard, P tempPlayer) //games like spades 2 player, they may or may not add to list.
    {
        thisCard.IsUnknown = false;
        tempPlayer.MainHandList.Add(thisCard);
        return Task.CompletedTask;
    }
    protected virtual bool CardToCurrentPile()
    {
        return true;
    }
    protected virtual bool ShowNewCardDrawn(P tempPlayer)
    {
        if (tempPlayer.PlayerCategory != EnumPlayerCategory.OtherHuman)
        {
            return true;
        }
        return false;
    }
    protected virtual async Task PlayerReceivesNoCardsAfterDrawingAsync(D thisCard)
    {
        _gameContainer.Command.UpdateAll();
        await Aggregator.AnimatePlayAsync(thisCard, () =>
        {
            if (_model.Pile1!.Visible == true)
            {
                _model.Pile1.AddCard(thisCard);
            }
        });
        await AfterDrawingAsync();
    }
    protected async Task ReshuffleCardsAsync(bool canSend) //needs to be protected so games like hit the deck can call into it when cutting deck.
    {
        var thisCol = GetReshuffleList();
        thisCol.ShuffleList();
        _model.Deck1!.OriginalList(thisCol);
        await MiddleReshuffleCardsAsync(thisCol, canSend);
    }
    protected virtual DeckRegularDict<D> GetReshuffleList() //games like milk run requires something else to get the list of cards to reshuffle.
    {
        var thisCol = _model.Pile1!.DiscardList();
        if (CardInfo!.ReshuffleAllCardsFromDiscard == true)
        {
            thisCol.Add(_model.Pile1.GetCardInfo());
            _model.Pile1.ClearCards();
        }
        foreach (var thisCard in thisCol)
        {
            thisCard.Reset();
        }
        return thisCol;
    }
    protected virtual async Task MiddleReshuffleCardsAsync(IDeckDict<D> thisList, bool canSend)
    {
        if (canSend == true)
        {
            BasicList<int> newList = thisList.ExtractIntegers(Items => Items.Deck);
            await Network!.SendAllAsync("reshuffledcards", newList);
        }
        await AfterReshuffleAsync();
    }
    protected virtual async Task AfterReshuffleAsync() //at this time, the cards should have been reshuffled.
    {
        if (CardInfo!.ReshuffleAllCardsFromDiscard)
        {
            _model!.Pile1!.ClearCards();
        }
        else
        {
            _model.Pile1!.CardsReshuffled();
        }
        if (DoDraw == true)
        {
            await DrawAsync();
            return;
        }
        bool hadComputer = SingleInfo!.PlayerCategory == EnumPlayerCategory.Computer;
        if (hadComputer == true)
        {
            SingleInfo = PlayerList!.GetSelf();
        }
        if (SingleInfo.PlayerCategory == EnumPlayerCategory.OtherHuman)
        {
            await ContinueTurnAsync();
        }
        if (hadComputer == true)
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
        }
    }
    protected virtual async Task AfterDrawingAsync()
    {
        await ContinueTurnAsync();
    }
    protected virtual Task SetSaveRootObjectsAsync() { return Task.CompletedTask; }
    public override async Task SetUpGameAsync(bool isBeginning)
    {
        await StartShufflingCardsAsync(isBeginning);
    }
    private async Task StartShufflingCardsAsync(bool isBeginning)
    {
        _gameContainer.DeckList!.ClearObjects(); //because it needs to shuffle all.
        _gameContainer.DeckList.ShuffleObjects();
        await StartSetUpAsync(isBeginning);
    }
    protected virtual bool AutoOtherPileCurrentOnly()
    {
        return true;
    }
    protected virtual async Task StartSetUpAsync(bool isBeginning)
    {
        if (_model.OtherPile != null)
        {
            if (AutoOtherPileCurrentOnly())
            {
                _model.OtherPile.CurrentOnly = true;
            }
            _model.OtherPile.ClearCards();
        }
        if (FinishUpAsync == null)
        {
            throw new CustomBasicException("The loader never set the finish up code.  Rethink");
        }
        _model.Pile1!.ClearCards();
        _model.Deck1!.ClearCards();
        DeckRegularDict<D>? tempList = default;
        SaveRoot!.PreviousCard = 0;
        _gameContainer.AlreadyDrew = false;
        if (CardInfo!.NoPass == false)
        {
            if (CardInfo.CardsToPassOut == 0)
            {
                throw new CustomBasicException("Cannot have 0 cards to pass out.  If there are truly 0 cards to pass out; then NoPass will be True");
            }
            if (CardInfo.NeedsDummyHand == true)
            {
                PlayerList!.AddDummy();
            }
            DeckRegularDict<D> firstList = _gameContainer.DeckList!.ToRegularDeckDict();
            CardInfo.PlayerExcludeList.ForEach(items =>
            {
                if (firstList.ObjectExist(items))
                {
                    firstList.RemoveObjectByDeck(items);
                }
            });
            int cardsToPassOut;
            if (Test!.CardsToPass > 0)
            {
                cardsToPassOut = Test.CardsToPass;
            }
            else
            {
                cardsToPassOut = CardInfo.CardsToPassOut;
            }
            if (CardInfo.PassOutAll == false)
            {
                bool rets;
                int testCount = 0;
                rets = MainContainer.RegistrationExist<ITestCardSetUp<D, P>>();
                if (rets == true)
                {
                    if (BasicData!.GamePackageMode == EnumGamePackageMode.Production)
                    {
                        throw new CustomBasicException("Cannot have test hands because its production.");
                    }
                    ITestCardSetUp<D, P> testPass = MainContainer.Resolve<ITestCardSetUp<D, P>>();
                    await testPass.SetUpTestHandsAsync(PlayerList!, _gameContainer.DeckList!);
                    PlayerList!.ForEach(items =>
                    {
                        testCount += items.StartUpList.Count;
                        items.StartUpList.ForEach(Card =>
                        {
                            firstList.RemoveObjectByDeck(Card.Deck); //because its already in hand.
                        });
                    });
                }
                ps.CardProcedures.PassOutCards(PlayerList!, firstList, cardsToPassOut, testCount, Test.ComputerNoCards, ref tempList!);
            }
            else
            {
                ps.CardProcedures.PassOutCards(PlayerList!, firstList, Test.ComputerNoCards);
            }
            if (CardInfo.NeedsDummyHand == true)
            {
                CardInfo.DummyHand = PlayerList!["dummy"].MainHandList;
                PlayerList.RemoveDummy();
            }
        }
        else if (CardInfo.NeedsDummyHand == true)
        {
            throw new CustomBasicException("Cannot require dummy hand because no cards are even being passed out");
        }
        if (CardInfo.NoPass == false && CardInfo.PassOutAll == false)
        {
            _model.Deck1!.OriginalList(tempList!);
        }
        else if (CardInfo.PassOutAll == false)
        {
            _model.Deck1!.OriginalList(_gameContainer.DeckList!);
        }
        if (_gameContainer.GameInfo!.SinglePlayerChoice != EnumPlayerChoices.HumanOnly)
        {
            SingleInfo = PlayerList!.GetSelf();
        }
        if (CardInfo.NoPass == false)
        {
            if (_model.PlayerHand1!.Visible == true)
            {
                SetHand();
                SetUpSelfHand(); //usually nothing but games like fluxx can do other things.
                PrepSort();
                if (CardInfo.CanSortCardsToBeginWith == true)
                {
                    SortCards();
                    if (CardInfo.NeedsDummyHand == true)
                    {
                        if (_thisSort == null)
                        {
                            CardInfo.DummyHand.Sort();
                        }
                        else
                        {
                            CardInfo.DummyHand.Sort(_thisSort);
                        }
                    }
                }
            }
            else
            {
                LinkHand();
            }
        }
        else
        {
            if (CardInfo.PlayerGetsCards)
            {
                SetHand(); //even if not passing, needs to hook up because players can get cards later (like ya blew it).
            }
            LinkHand();
        }
        if (CardInfo.AddToDiscardAtBeginning == true && _model.Pile1!.Visible == true && CardInfo.NoPass == false)
        {
            if (Test!.AutoNearEndOfDeckBeginning == false)
            {
                var list = CardInfo.DiscardExcludeList(_gameContainer.DeckList);
                if (list.Count == 0)
                {
                    _model.Pile1.AddCard(_model.Deck1!.DrawCard()); //drawing a card for deck
                }
                else
                {
                    int deck = _model.Deck1.RevealCard().Deck;
                    if (list.Contains(deck) == false)
                    {
                        _model.Pile1.AddCard(_model.Deck1!.DrawCard());
                    }
                    else
                    {
                        var nextList = _model.Deck1.SavedList();
                        var temps = nextList.Select(x => x.Deck).ToBasicList();
                        temps.RemoveGivenList(list);
                        deck = temps.First();
                        if (list.Contains(deck))
                        {
                            throw new CustomBasicException("Cannot contain card because on exclude list.  Rethink");
                        }
                        _model.Pile1.AddCard(_model.Deck1!.DrawCard(deck));
                    }
                }
            }
            else
            {
                int suggested = _model.Deck1!.CardsLeft() - 2;
                var flist = _model.Deck1.DrawSeveralCards(suggested);
                _model.Pile1.AddSeveralCards(flist);
            }
        }
        else if (Test!.AutoNearEndOfDeckBeginning)
        {
            int suggested = _model.Deck1!.CardsLeft() - 2;
            _model.Deck1.DrawSeveralCards(suggested); //so even if no discards, can still draw several cards when necessary
        }
        await LastPartOfSetUpBeforeBindingsAsync();
        await FinishUpAsync.Invoke(isBeginning);
    }
    protected virtual void LinkHand() { }
    protected virtual void SetUpSelfHand() { }
    protected virtual void SetHand()
    {
        _model.PlayerHand1!.HandList = SingleInfo!.MainHandList;
    }
    protected virtual Task LastPartOfSetUpBeforeBindingsAsync() { return Task.CompletedTask; }
    private ISortObjects<D>? _thisSort;
    protected void PrepSort() //games like spades may need to call this so it can link up and still sort.
    {
        bool rets;
        rets = MainContainer.RegistrationExist<ISortObjects<D>>();
        if (rets == true)
        {
            _thisSort = MainContainer.Resolve<ISortObjects<D>>();
        }
    }
    public void SortCards()
    {
        if (_thisSort != null)
        {
            _model.PlayerHand1!.HandList.Sort(_thisSort);
        }
        else
        {
            _model.PlayerHand1!.HandList.Sort();
        }
    }
    protected void SortCards(IDeckDict<D> thisList) //this is needed for games like cribbage.
    {
        if (_thisSort != null)
        {
            thisList.Sort(_thisSort);
        }
        else
        {
            thisList.Sort();
        }
    }
    protected virtual Task PlayerChosenForPickingUpFromDiscardAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        return Task.CompletedTask;
    }
    protected virtual async Task AfterPickupFromDiscardAsync()
    {
        await ContinueTurnAsync();
    }
    protected virtual async Task AnimatePickupAsync(D card)
    {
        if (_model.OtherPile != null && Test!.NoAnimations == false)
        {
            await Aggregator.AnimateCardAsync(card, EnumAnimcationDirection.StartUpToCard, "otherpile");
        }
        else if (Test!.NoAnimations == false)
        {
            await Aggregator.AnimatePickUpDiscardAsync(card);
        }
    }
    public virtual async Task PickupFromDiscardAsync()
    {
        await PlayerChosenForPickingUpFromDiscardAsync();
        if (SingleInfo!.CanSendMessage(BasicData!) == true)
        {
            await Network!.SendAllAsync("pickup");
        }
        var thisCard = _model.Pile1!.GetCardInfo();
        _gameContainer.AlreadyDrew = true;
        SaveRoot!.PreviousCard = thisCard.Deck;
        _model.Pile1.RemoveFromPile();
        await AnimatePickupAsync(thisCard);
        if (_model.OtherPile != null)
        {
            _model.OtherPile.AddCard(thisCard);
            await ContinueTurnAsync();
            return;
        }
        thisCard.Drew = true;
        SingleInfo!.MainHandList.Add(thisCard);
        SortCards();
        await AfterPickupFromDiscardAsync();
    }
    protected async Task<DeckRegularDict<D>> PopulateCardsFromTurnHandAsync(string data)
    {
        BasicList<int> firstList = await js.DeserializeObjectAsync<BasicList<int>>(data);
        DeckRegularDict<D> output = new();
        firstList.ForEach(index =>
        {
            D card = SingleInfo!.MainHandList.GetSpecificItem(index);
            output.Add(card);
        });
        return output;
    }
    async Task IReshuffledCardsNM.ReshuffledCardsReceived(string data)
    {
        if (CardInfo!.ShowMessageWhenReshuffling == true)
        {
            _toast.ShowInfoToast("Its the end of the deck; therefore; the cards are being reshuffled");
        }
        BasicList<int> firstList = await js.DeserializeObjectAsync<BasicList<int>>(data);
        DeckRegularDict<D> newList = new();
        firstList.ForEach(index =>
        {
            D card = new();
            card.Populate(index);
            newList.Add(card);
            _gameContainer.DeckList!.RelinkObject(index, card);
        });
        _model.Deck1!.OriginalList(newList);
        await AfterReshuffleAsync();
    }
    async Task IDrawCardNM.DrawCardReceivedAsync(string data)
    {
        await SentDrawCardAsync(data);
    }
    protected virtual async Task SentDrawCardAsync(string data)
    {
        LeftToDraw = 0;
        PlayerDraws = 0;
        SingleInfo = PlayerList!.GetWhoPlayer();
        await DrawAsync();
    }
    async Task IPickUpNM.PickUpReceivedAsync(string data)
    {
        await PickupFromDiscardAsync();
    }
    async Task IDiscardNM.DiscardReceivedAsync(string data)
    {
        await DiscardAsync(int.Parse(data));
    }
}