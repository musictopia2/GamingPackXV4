namespace HitTheDeck.Core.Logic;
[SingletonGame]
public class HitTheDeckMainGameClass
    : CardGameClass<HitTheDeckCardInformation, HitTheDeckPlayerItem, HitTheDeckSaveInfo>
    , IMiscDataNM, IStartNewGame, ISerializable
{
    private readonly HitTheDeckVMData _model;
    private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
    private readonly HitTheDeckGameContainer _gameContainer; //if we don't need it, take it out.
    private readonly IToast _toast;

    public HitTheDeckMainGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        HitTheDeckVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<HitTheDeckCardInformation> cardInfo,
        CommandContainer command,
        HitTheDeckGameContainer gameContainer,
        ISystemError error,
        IToast toast
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, error, toast)
    {
        _model = currentMod;
        _command = command;
        _gameContainer = gameContainer;
        _toast = toast;
    }
    private HitTheDeckCardInformation ManuallyChooseCard
    {
        get
        {
            var tempList = _gameContainer.DeckList.Where(items => items.CardType != EnumTypeList.Flip).ToRegularDeckDict();
            do
            {
                var thisCard = tempList.GetRandomItem();
                if (_model!.Deck1!.CardExists(thisCard.Deck))
                {
                    return thisCard;
                }
            } while (true);
        }
    }
    public override Task FinishGetSavedAsync()
    {
        LoadControls();
        SaveRoot!.LoadMod(_model!);
        return base.FinishGetSavedAsync();
    }
    private void LoadControls()
    {
        if (IsLoaded == true)
        {
            return;
        }
        IsLoaded = true;
    }
    protected override Task LastPartOfSetUpBeforeBindingsAsync()
    {
        var tempCard = ManuallyChooseCard;
        _model!.Deck1!.ManuallyRemoveSpecificCard(tempCard);
        _model.Pile1!.AddCard(tempCard);
        SaveRoot!.LoadMod(_model!);
        SaveRoot.ImmediatelyStartTurn = true;
        return base.LastPartOfSetUpBeforeBindingsAsync();
    }
    private int ComputerToPlay()
    {
        var thisList = SingleInfo!.MainHandList.Where(items => CanPlay(items.Deck)).ToRegularDeckDict();
        if (thisList.Count == 0)
        {
            return 0;
        }
        return thisList.GetRandomItem().Deck;
    }
    protected override async Task ComputerTurnAsync()
    {
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelaySeconds(.75);
        }
        SingleInfo = PlayerList!.GetWhoPlayer();
        var thisCard = _model!.Pile1!.GetCardInfo();
        if (thisCard.Instructions == EnumInstructionList.Flip)
        {
            if (BasicData!.MultiPlayer)
            {
                await Network!.SendAllAsync("flipdeck");
            }
            await FlipDeckAsync();
            return;
        }
        if (thisCard.Instructions == EnumInstructionList.Cut)
        {
            await CutDeckAsync();
            return;
        }
        if (_gameContainer.AlreadyDrew == true)
        {
            thisCard = SingleInfo!.MainHandList.Single(items => items.Drew);
            if (CanPlay(thisCard.Deck))
            {
                await ProcessPlayAsync(thisCard.Deck);
                return;
            }
            if (BasicData!.MultiPlayer == true)
            {
                await Network!.SendEndTurnAsync();
            }
            await EndTurnAsync();
            return;
        }
        int decks = ComputerToPlay();
        if (decks > 0)
        {
            await ProcessPlayAsync(decks);
            return;
        }
        SingleInfo!.MainHandList.UnhighlightObjects();
        if (BasicData!.MultiPlayer == true)
        {
            await Network!.SendDrawAsync();
        }
        await DrawAsync();
    }
    private async Task RandomDraw4Async()
    {
        PlayerDraws = _gameContainer.Random.GetRandomNumber(PlayerList.Count);
        var tempPlayer = PlayerList![PlayerDraws];
        if (BasicData!.MultiPlayer == true)
        {
            await Network!.SendAllAsync("playerdraw", PlayerDraws);
        }
        _toast.ShowInfoToast($"{tempPlayer.NickName} will be drawing 4 cards");
        SaveRoot!.HasDrawn = true;
        LeftToDraw = 4;
        await DrawAsync();
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        LoadControls();
        return base.StartSetUpAsync(isBeginning);
    }

    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status) //can't do switch because we don't know what the cases are ahead of time.
        {
            case "playerdraw":
                LeftToDraw = 4;
                PlayerDraws = int.Parse(content);
                var tempPlayer = PlayerList![PlayerDraws];
                _toast.ShowInfoToast($"{tempPlayer.NickName} will be drawing 4 cards");
                SaveRoot!.HasDrawn = true;
                await DrawAsync();
                break;
            case "cutdeck":
                await CutDeckAsync();
                return;
            case "flipdeck":
                await FlipDeckAsync();
                return;
            case "preparecut":
                _model.Deck1.IsCutting = true;
                DoDraw = false; //try this too.
                Network!.IsEnabled = true; //wait for next message
                return;
            case "play":
                await ProcessPlayAsync(int.Parse(content));
                return;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override async Task StartNewTurnAsync()
    {
        _model.Deck1.IsCutting = false; //try this way.
        await base.StartNewTurnAsync();
        PlayerDraws = 0;
        var newTurn = await PlayerList!.CalculateWhoTurnAsync();
        var newPlayer = PlayerList[newTurn];
        SaveRoot!.NextPlayer = newPlayer.NickName;
        SingleInfo = PlayerList.GetWhoPlayer();
        var thisCard = _model!.Pile1!.GetCardInfo();
        if (thisCard.Instructions == EnumInstructionList.Flip && SaveRoot.WasFlipped == false)
        {
            SaveRoot.WasFlipped = true;
        }
        else
        {
            SaveRoot.WasFlipped = false;
        }
        if (thisCard.Instructions == EnumInstructionList.RandomDraw && SaveRoot.HasDrawn == false)
        {
            int previousTurn = await PlayerList.CalculateOldTurnAsync();
            SingleInfo = PlayerList[previousTurn];
            if (BasicData!.MultiPlayer == true)
            {
                if (SingleInfo.CanSendMessage(BasicData) == false)
                {
                    Network!.IsEnabled = true;
                    return;
                }
            }
            await RandomDraw4Async();
            return;
        }
        await ContinueTurnAsync();
    }
    public override async Task EndTurnAsync()
    {
        ReconcileCards();
        SingleInfo = PlayerList!.GetWhoPlayer();
        SingleInfo.MainHandList.UnhighlightObjects();
        if (SingleInfo.PlayerCategory == EnumPlayerCategory.Computer)
        {
            SingleInfo.MainHandList.ForEach(thisCard => thisCard.Drew = false);
        }
        _command.ManuelFinish = true; //because it could be somebody else's turn.
        WhoTurn = await PlayerList.CalculateWhoTurnAsync();
        await StartNewTurnAsync();
    }
    public bool CanPlay(int deck)
    {
        if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self && Test!.AllowAnyMove)
        {
            return true;
        }
        var oldCard = _model!.Pile1!.GetCardInfo();
        var newCard = _gameContainer.DeckList!.GetSpecificItem(deck);
        if (newCard.Instructions == EnumInstructionList.Flip && _model.Deck1!.CanFlipDeck() == false)
        {
            return false;
        }
        if (oldCard.Instructions == EnumInstructionList.RandomDraw)
        {
            return oldCard.FirstSort == newCard.FirstSort || newCard.AnyColor;
        }
        if (oldCard.Instructions == EnumInstructionList.None)
        {
            return oldCard.FirstSort == newCard.FirstSort || oldCard.Number == newCard.Number || newCard.AnyColor;
        }
        if (oldCard.Instructions == EnumInstructionList.Flip || oldCard.Instructions == EnumInstructionList.Cut)
        {
            throw new CustomBasicException("Cannot ever play a card from your cards if the instructions is flip or cut the deck");
        }
        if (oldCard.Instructions == EnumInstructionList.PlayColor)
        {
            if (newCard.AnyColor)
            {
                return false;
            }
            return newCard.FirstSort == oldCard.FirstSort;
        }
        if (oldCard.Instructions == EnumInstructionList.PlayNumber)
        {
            if (newCard.Instructions != EnumInstructionList.None)
            {
                return false;
            }
            return newCard.Number == oldCard.Number;
        }
        throw new CustomBasicException("Cannot find out whether to play the card or not");
    }
    public async Task ProcessPlayAsync(int deck)
    {
        if (SingleInfo!.MainHandList.ObjectExist(deck))
        {
            SingleInfo.MainHandList.RemoveObjectByDeck(deck);
        }
        if (SingleInfo.CanSendMessage(BasicData!))
        {
            await Network!.SendAllAsync("play", deck);
        }
        var thisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
        thisCard.Drew = false;
        await AnimatePlayAsync(thisCard);
        thisCard = _model!.Pile1!.GetCardInfo();
        SaveRoot!.HasDrawn = false;
        if (thisCard.Deck != deck)
        {
            throw new CustomBasicException("This is not the card played.  That means there is a problem (possibly with the discard)");
        }
        if (SingleInfo.MainHandList.Count == 0)
        {
            await EndRoundAsync();
            return;
        }
        await EndTurnAsync();
    }
    private void ReconcileCards()
    {
        int deckcount = _model!.Deck1!.CardsLeft();
        int pileCount = _model.Pile1!.CardsLeft();
        int counts = deckcount + pileCount;
        PlayerList!.ForEach(thisPlayer =>
        {
            counts += thisPlayer.MainHandList.Count;
        });
        if (counts != 110)
        {
            throw new CustomBasicException($"Wrong card count upon ending turn.  Accounted for {counts} and not 110");
        }
    }
    private void ReconcileFlips(DeckRegularDict<HitTheDeckCardInformation> discardList, DeckRegularDict<HitTheDeckCardInformation> deckList)
    {
        DeckRegularDict<HitTheDeckCardInformation> finalList = new();
        finalList.AddRange(discardList);
        finalList.AddRange(deckList);
        PlayerList!.ForEach(thisPlayer =>
        {
            finalList.AddRange(thisPlayer.MainHandList);
        });
        110.Times(x =>
        {
            if (finalList.ObjectExist(x) == false)
            {
                throw new CustomBasicException($"Deck of {x} is not accounted for");
            }
        });
        if (finalList.Count != 110)
        {
            throw new CustomBasicException($"There has to be a total of 110 cards after playing flip, not {finalList.Count}");
        }
    }
    public async Task FlipDeckAsync()
    {
        _command.ManuelFinish = true;
        var deckList = _model!.Deck1!.FlipCardList();
        PlayerList!.ChangeReverse();
        var discardList = _model.Pile1!.FlipCardList();
        ReconcileFlips(discardList, deckList);
        _model.Deck1.OriginalList(discardList);
        _model.Pile1.NewList(deckList);
        ReconcileCards();
        var thisCard = _model.Pile1.GetCardInfo();
        bool useDeck;
        if (thisCard.Instructions == EnumInstructionList.Flip)
        {
            useDeck = !_model.Pile1.CanCutDiscard();
            do
            {
                if (useDeck == false)
                {
                    _model.Pile1.CutDeck();
                }
                else
                {
                    _model.Deck1.PutInMiddle(_model.Pile1.GetCardInfo());
                    _model.Pile1.RemoveFromPile();
                }
                thisCard = _model.Pile1.GetCardInfo();
                if (thisCard.Instructions != EnumInstructionList.Flip)
                {
                    thisCard.IsUnknown = false;
                    break;
                }
            } while (true);
        }
        await EndTurnAsync();
    }
    public override async Task ContinueTurnAsync()
    {
        await base.ContinueTurnAsync();
        _command.ManualReport();
    }
    private bool CanProcessPlay
    {
        get
        {
            if (BasicData!.MultiPlayer == false)
            {
                return true;
            }
            return SingleInfo!.CanSendMessage(BasicData);
        }
    }
    public async Task CutDeckAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        if (CanProcessPlay)
        {
            _model.Deck1.IsCutting = true;
            if (_model!.Deck1!.CanCutDeck() == false)
            {
                if (BasicData.MultiPlayer)
                {
                    await Network!.SendAllAsync("preparecut");
                }
                var deckList = _model.Deck1.FlipCardList();
                _toast.ShowInfoToast("Its the end of the deck while cutting. therefore; the cards are being reshuffled");
                _model.Pile1!.AddRestOfDeck(deckList);
                await ReshuffleCardsAsync(SingleInfo!.CanSendMessage(BasicData!)); //try this way.
            }
            if (BasicData!.MultiPlayer == true)
            {
                await Network!.SendAllAsync("cutdeck");
            }
        }
        _model!.Pile1!.AddCard(_model.Deck1!.CutTheDeck());
        await EndTurnAsync();
    }
    private void UpdateScores()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.PreviousPoints = thisPlayer.MainHandList.Sum(items => items.Points);
            thisPlayer.TotalPoints += thisPlayer.PreviousPoints;
        });
    }
    public override async Task EndRoundAsync()
    {
        SaveRoot!.NextPlayer = "None";
        UpdateScores();
        if (CanEndGame == false)
        {
            await this.RoundOverNextAsync();
            return;
        }
        SingleInfo = PlayerList.OrderBy(items => items.TotalPoints).First();
        await ShowWinAsync();
    }
    Task IStartNewGame.ResetAsync()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.TotalPoints = 0;
            thisPlayer.PreviousPoints = 0;
        });
        return Task.CompletedTask;
    }
    private bool CanEndGame
    {
        get
        {
            int pointsNeeded;
            if (PlayerList.Count == 2)
            {
                pointsNeeded = 70;
            }
            else
            {
                pointsNeeded = 100;
            }
            return PlayerList.Any(items => items.TotalPoints >= pointsNeeded);
        }
    }
}