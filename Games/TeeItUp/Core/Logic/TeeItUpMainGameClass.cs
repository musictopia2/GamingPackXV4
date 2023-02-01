namespace TeeItUp.Core.Logic;
[SingletonGame]
public class TeeItUpMainGameClass
    : CardGameClass<TeeItUpCardInformation, TeeItUpPlayerItem, TeeItUpSaveInfo>
    , IMiscDataNM, IStartNewGame, ISerializable
{
    private readonly TeeItUpVMData _model;
    private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
    private readonly TeeItUpGameContainer _gameContainer; //if we don't need it, take it out.
    private readonly ISystemError _error;
    private readonly IToast _toast;
    public TeeItUpMainGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        TeeItUpVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<TeeItUpCardInformation> cardInfo,
        CommandContainer command,
        TeeItUpGameContainer gameContainer,
        ISystemError error,
        IToast toast
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, error, toast)
    {
        _model = currentMod;
        _command = command;
        _gameContainer = gameContainer;
        _error = error;
        _toast = toast;
        _gameContainer.BoardClickedAsync = BoardClicked;
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
    protected override async Task ComputerTurnAsync()
    {
        await Task.CompletedTask;
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        LoadControls();
        SaveRoot!.Round++;
        SaveRoot.LoadMod(_model!);
        SaveRoot.ImmediatelyStartTurn = true;
        SaveRoot.Begins = WhoTurn;
        SaveRoot.GameStatus = EnumStatusType.Beginning;
        return base.StartSetUpAsync(isBeginning);
    }
    public override Task ContinueTurnAsync()
    {
        if (SaveRoot.GameStatus == EnumStatusType.Beginning)
        {
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
            {
                _model.Instructions = "Please choose the 2 cards to turn face up";
            }
            else
            {
                _model.Instructions = $"Waiting for {SingleInfo.NickName} to choose the 2 cards to turn face up";
            }
        }
        return base.ContinueTurnAsync();
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "firstcard":
                SendPlay send = await GetSentPlayAsync(content);
                await BeginningCardSelectedAsync(send.Deck, send.Player);
                return;
            case "firstmulliganchosen":
                await UseMulliganFirstTurnAsync(int.Parse(content));
                return;
            case "firstmulliganplayed":
                var thisSend2 = await GetSentPlayAsync(content);
                await FirstMulliganTradeAsync(thisSend2.Player, thisSend2.Deck);
                return;
            case "stealcard":
                var thisSend3 = await GetSentPlayAsync(content);
                await StealCardAsync(thisSend3.Player, thisSend3.Deck);
                return;
            case "tradecard":
                await TradeCardAsync(int.Parse(content));
                return;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    private bool HasGoneOut
    {
        get
        {
            return SingleInfo!.PlayerBoard!.IsFinished;
        }
    }
    public override async Task EndRoundAsync()
    {
        CalculateScore();
        if (SaveRoot!.Round == 9)
        {
            SingleInfo = PlayerList.OrderBy(items => items.TotalScore).First();
            await ShowWinAsync();
            return;
        }
        await this.RoundOverNextAsync();
    }
    private void CalculateScore()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            var finalList = thisPlayer.PlayerBoard!.GetFinalCards();
            thisPlayer.PreviousScore = finalList.Sum(items => items.Points);
            thisPlayer.TotalScore += thisPlayer.PreviousScore;
        });
    }
    public override async Task StartNewTurnAsync()
    {
        await base.StartNewTurnAsync();
        SaveRoot!.FirstMulligan = false;
        _gameContainer.PreviousCard = 0;
        await ContinueTurnAsync();
    }
    public override async Task EndTurnAsync()
    {
        PlayerList!.ForEach(tempPlayer =>
        {
            tempPlayer.PlayerBoard!.DoubleCheck();
        });
        SingleInfo = PlayerList!.GetWhoPlayer();
        SingleInfo.MainHandList.UnhighlightObjects();
        if (SaveRoot!.GameStatus == EnumStatusType.Normal)
        {
            if (HasGoneOut == true)
            {
                _toast.ShowInfoToast($"{SingleInfo.NickName} has gone out.  Therefore, everyone gets one last turn");
                SingleInfo.WentOut = true;
                _model.Instructions = "Last Turn";
                SaveRoot.Begins = WhoTurn;
                SaveRoot.GameStatus = EnumStatusType.Finished;
            }
        }
        _command.ManuelFinish = true; //because it could be somebody else's turn.
        WhoTurn = await PlayerList.CalculateWhoTurnAsync();
        if (SaveRoot.GameStatus == EnumStatusType.Finished && WhoTurn == SaveRoot.Begins)
        {
            await EndRoundAsync();
            return;
        }
        if (SaveRoot.GameStatus == EnumStatusType.FirstTurn && WhoTurn == SaveRoot.Begins)
        {
            SaveRoot.GameStatus = EnumStatusType.Normal;
        }
        await StartNewTurnAsync();
    }
    public override Task PopulateSaveRootAsync()
    {
        PlayerList.ForEach(player =>
        {
            if (player.PlayerBoard != null)
            {
                player.MainHandList = player.PlayerBoard!.ObjectList.ToRegularDeckDict();
            }
        });
        return base.PopulateSaveRootAsync();
    }
    protected override Task LastPartOfSetUpBeforeBindingsAsync()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.MainHandList.ForEach(card =>
            {
                card.IsUnknown = true;
                card.IsSelected = false;
                card.Visible = true;
                card.MulliganUsed = false;
            });
            thisPlayer.FinishedChoosing = false;
            thisPlayer.WentOut = false;
        });
        return base.LastPartOfSetUpBeforeBindingsAsync();
    }
    internal async Task BoardClicked(TeeItUpPlayerItem thisPlayer, TeeItUpCardInformation thisCard)
    {
        int playerID = thisPlayer.Id;
        int deck = thisCard.Deck;
        try
        {
            if (SaveRoot!.GameStatus == EnumStatusType.Beginning)
            {
                if (thisPlayer.PlayerBoard!.CanStart)
                {
                    throw new CustomBasicException("Should have not allowed this since 2 cards was already selected");
                }
                await BeginningCardSelectedAsync(deck, playerID);
                return;
            }
            bool knowns = thisPlayer.PlayerBoard!.IsCardKnown(deck);
            if (thisCard.IsMulligan == true && knowns == true)
            {
                if (SaveRoot.GameStatus != EnumStatusType.FirstTurn)
                {
                    _toast.ShowUserErrorToast("This is not the first turn.  Therefore, the Mulligan card is not valid");
                    return;
                }
                if (_model!.OtherPile!.PileEmpty() == false)
                {
                    _toast.ShowUserErrorToast("Sorry, since you already drew, cannot use the Mulligan");
                    return;
                }
                if (thisPlayer.PlayerBoard.IsSelf == false)
                {
                    _toast.ShowUserErrorToast("Sorry, cannot use someone else's Mulligan");
                    return;
                }
                if (thisPlayer.PlayerBoard.IsMulliganValid(deck) == false)
                {
                    _toast.ShowUserErrorToast("Sorry, the mulligan cannot be used because someone traded with you");
                    return;
                }
                if (BasicData!.MultiPlayer == true)
                {
                    await Network!.SendAllAsync("firstmulliganchosen", deck);
                }
                await UseMulliganFirstTurnAsync(deck);
                return;
            }
            bool selfs = thisPlayer.PlayerBoard.IsSelf;
            if (SaveRoot.FirstMulligan)
            {
                if (selfs)
                {
                    _toast.ShowUserErrorToast("Since you are using a Mulligan, you must trade with another player");
                    return;
                }
                if (thisPlayer.PlayerBoard.CanStealCard(deck) == false)
                {
                    _toast.ShowUserErrorToast("The card chosen cannot be stolen");
                    return;
                }
                if (BasicData!.MultiPlayer == true)
                {
                    await SendPlayAsync("firstmulliganplayed", deck, playerID);
                }
                await FirstMulliganTradeAsync(playerID, deck);
                return;
            }
            int oldDeck;
            if (_model!.OtherPile!.PileEmpty() == true)
            {
                oldDeck = 0;
            }
            else
            {
                oldDeck = _model.OtherPile.GetCardInfo().Deck;
            }
            if (thisCard.IsMulligan && knowns)
            {
                _toast.ShowUserErrorToast("A mulligan card cannot be used for trading");
                return;
            }
            if (oldDeck == 0 && selfs == false)
            {
                _toast.ShowUserErrorToast("Cannot steal any card because no card is chosen");
                return;
            }
            if (selfs == false)
            {
                var previousCard = _model.OtherPile.GetCardInfo();
                if (previousCard.IsMulligan == false)
                {
                    _toast.ShowUserErrorToast("Since the card is not a mulligan, cannot steal another player's card");
                    return;
                }
                if (thisPlayer.PlayerBoard.CanStealCard(deck) == false)
                {
                    _toast.ShowUserErrorToast("The card cannot be stolen from another player");
                    return;
                }
                if (BasicData!.MultiPlayer == true)
                {
                    await SendPlayAsync("stealcard", deck, playerID);
                }
                await StealCardAsync(playerID, deck);
                return;
            }
            if (oldDeck == 0)
            {
                _toast.ShowUserErrorToast("Sorry, you must choose to pickup from discard or the deck first");
                return;
            }
            if (thisPlayer.PlayerBoard.IsPartFrozen(deck))
            {
                _toast.ShowUserErrorToast("Since the column is frozen, cannot trade it anymore for the round");
                return;
            }
            if (HasMatch(thisPlayer.PlayerBoard, deck, knowns) == false) //hopefully this is still correct.
            {
                _toast.ShowUserErrorToast("There is a match.  Therefore, must return a match");
                return;
            }
            if (BasicData!.MultiPlayer == true)
            {
                await Network!.SendAllAsync("tradecard", deck);
            }
            await TradeCardAsync(deck);
        }
        catch (Exception ex)
        {
            _error.ShowSystemError(ex.Message);
        }
    }
    public async Task SendPlayAsync(string status, int deck, int player)
    {
        SendPlay thisSend = new();
        thisSend.Deck = deck;
        thisSend.Player = player;
        await Network!.SendAllAsync(status, thisSend);
    }
    public static async Task<BasicList<SendPlay>> GetSentListAsync(string message)
    {
        return await js1.DeserializeObjectAsync<BasicList<SendPlay>>(message);
    }
    public static async Task<SendPlay> GetSentPlayAsync(string message)
    {
        return await js1.DeserializeObjectAsync<SendPlay>(message);
    }
    Task IStartNewGame.ResetAsync()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.PreviousScore = 0;
            thisPlayer.TotalScore = 0;
        });
        SaveRoot!.Round = 0;
        return Task.CompletedTask;
    }
    public override Task DiscardAsync(TeeItUpCardInformation ThisCard)
    {
        ThisCard.IsUnknown = false;
        return base.DiscardAsync(ThisCard);
    }
    public async Task BeginningCardSelectedAsync(int deck, int player)
    {
        SingleInfo = PlayerList![player];
        SingleInfo.PlayerBoard!.ChooseCard(deck);
        if (SingleInfo.CanSendMessage(BasicData))
        {
            SendPlay send = new()
            {
                Deck = deck,
                Player = player
            };
            await Network!.SendAllAsync("firstcard", send);
        }
        if (SingleInfo.PlayerBoard.CanStart == false)
        {
            await ContinueTurnAsync();
            return;
        }
        SingleInfo.FinishedChoosing = true;
        if (PlayerList.Any(items => items.FinishedChoosing == false))
        {
            await EndTurnAsync();
            return;
        }
        _command.ManuelFinish = true;
        SaveRoot!.GameStatus = EnumStatusType.FirstTurn;
        _model.NormalTurn = "None";
        _model.Instructions = "None";
        _command.ManuelFinish = true; //because it could be somebody else's turn.
        WhoTurn = await PlayerList.CalculateWhoTurnAsync();
        await StartNewTurnAsync();
    }
    public async Task FirstMulliganTradeAsync(int player, int deck)
    {
        if (SaveRoot!.FirstMulligan == false)
        {
            throw new CustomBasicException("There was no first mulligan");
        }
        TeeItUpPlayerItem thisPlayer = PlayerList![player];
        thisPlayer.PlayerBoard!.TradeCard(deck, _model!.OtherPile!.GetCardInfo().Deck);
        SingleInfo = PlayerList.GetWhoPlayer();
        SingleInfo.PlayerBoard!.ReplaceFirstWith(deck);
        _model.OtherPile.ClearCards();
        await EndTurnAsync();
    }
    public async Task UseMulliganFirstTurnAsync(int deck)
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        SingleInfo.PlayerBoard!.UseFirstMulligan(deck);
        TeeItUpCardInformation thisCard = new();
        thisCard.Populate(deck);
        _model!.OtherPile!.AddCard(thisCard);
        SaveRoot!.FirstMulligan = true;
        await ContinueTurnAsync();
    }
    public async Task StealCardAsync(int player, int deck)
    {
        TeeItUpPlayerItem thisPlayer = PlayerList![player];
        thisPlayer.PlayerBoard!.TradeCard(deck, _model!.OtherPile!.GetCardInfo().Deck);
        var thisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
        _model.OtherPile.AddCard(thisCard);
        await ContinueTurnAsync();
    }
    public bool HasMatch(TeeItUpPlayerBoardCP thisBoard, int decks, bool isKnowns)
    {
        int matches = thisBoard.ColumnMatched(_model!.OtherPile!.GetCardInfo().Deck);
        if (matches == 0)
        {
            return true;
        }
        if (isKnowns == false)
        {
            return false;
        }
        var thisCard = _model.OtherPile.GetCardInfo();
        var newCard = _gameContainer.DeckList!.GetSpecificItem(decks);
        return thisCard.Points == newCard.Points;
    }
    public async Task TradeCardAsync(int deck)
    {
        try
        {
            int matches = SingleInfo!.PlayerBoard!.ColumnMatched(_model!.OtherPile!.GetCardInfo().Deck);
            if (HasMatch(SingleInfo.PlayerBoard, deck, true) == false || matches == 0)
            {
                SingleInfo.PlayerBoard.TradeCard(deck, _model.OtherPile.GetCardInfo().Deck);
                TeeItUpCardInformation tempCard = new();
                tempCard.Populate(deck);
                await DiscardAsync(tempCard);
                return;
            }
            SingleInfo.PlayerBoard.MatchCard(deck, out int newDeck);
            if (newDeck == 0)
            {
                _error.ShowSystemError("When trading card, newdeck cannot be 0");
            }
            TeeItUpCardInformation finCard = new();
            finCard.Populate(newDeck);
            await DiscardAsync(finCard);
        }
        catch (Exception ex)
        {
            _error.ShowSystemError(ex.Message);
        }
    }
}