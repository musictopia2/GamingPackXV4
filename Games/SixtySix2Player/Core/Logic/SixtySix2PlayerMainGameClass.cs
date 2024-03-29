namespace SixtySix2Player.Core.Logic;
[SingletonGame]
public class SixtySix2PlayerMainGameClass
    : TrickGameClass<EnumSuitList, SixtySix2PlayerCardInformation, SixtySix2PlayerPlayerItem, SixtySix2PlayerSaveInfo>
        , IMiscDataNM, IStartNewGame, ISerializable
{
    private readonly SixtySix2PlayerVMData _model;
    private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
    private readonly SixtySix2PlayerGameContainer _gameContainer; //if we don't need it, take it out.
    private readonly IAdvancedTrickProcesses _aTrick;
    private readonly IToast _toast;
    public SixtySix2PlayerMainGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        SixtySix2PlayerVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<SixtySix2PlayerCardInformation> cardInfo,
        CommandContainer command,
        SixtySix2PlayerGameContainer gameContainer,
        ITrickData trickData,
        ITrickPlay trickPlay,
        IAdvancedTrickProcesses aTrick,
        ISystemError error,
        IToast toast
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, trickData, trickPlay, error, toast)
    {
        _model = currentMod;
        _command = command;
        _gameContainer = gameContainer;
        _aTrick = aTrick;
        _toast = toast;
    }
    public override async Task FinishGetSavedAsync()
    {
        LoadControls();
        LoadVM();
        SaveRoot!.LoadMod(_model!);
        await base.FinishGetSavedAsync();
        _aTrick!.LoadGame();
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
        if (SingleInfo!.MainHandList.Count == 0)
        {
            throw new CustomBasicException("It should have gone to a new round automatically");
        }
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelaySeconds(.25);
        }
        var tempList = SingleInfo.MainHandList.Where(items => IsValidMove(items.Deck)).ToRegularDeckDict();
        await PlayCardAsync(tempList.GetRandomItem().Deck);
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        LoadControls();
        LoadVM();
        SaveRoot!.LoadMod(_model!);
        SaveRoot.LastTrickWon = 0;
        SaveRoot.CardList.Clear();
        _aTrick!.ClearBoard();
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.MarriageList.Clear();
            thisPlayer.FirstMarriage = EnumMarriage.None;
            thisPlayer.TricksWon = 0;
            thisPlayer.ScoreRound = 0;
        });
        return base.StartSetUpAsync(isBeginning);
    }
    protected override Task LastPartOfSetUpBeforeBindingsAsync()
    {
        SaveRoot!.TrumpSuit = _model!.Pile1!.GetCardInfo().Suit;
        return base.LastPartOfSetUpBeforeBindingsAsync();
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "exchangediscard":
                await ExchangeDiscardAsync(int.Parse(content));
                return;
            case "announcemarriage":
                BasicList<int> thisList = await js1.DeserializeObjectAsync<BasicList<int>>(content);
                await AnnounceMarriageAsync(thisList);
                return;
            case "goout":
                await GoOutAsync();
                return;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override async Task StartNewTurnAsync()
    {
        await base.StartNewTurnAsync();
        await ContinueTurnAsync();
    }
    public override async Task EndTurnAsync()
    {
        await Task.CompletedTask;
        throw new CustomBasicException("Should never have called the endturn.  Rethink");
    }
    public override async Task EndRoundAsync()
    {
        await Task.CompletedTask;
        throw new CustomBasicException("EndRound should not be called directly");
    }
    public override bool IsValidMove(int deck)
    {
        if (SaveRoot!.CardsForMarriage.Count > 0)
        {
            if (SaveRoot.CardsForMarriage.Any(items => items == deck) == false)
            {
                PlayErrorMessage = "Since a marriage was announced; must choose one of those cards to play";
                return false;
            }
        }
        if (_model!.Pile1!.PileEmpty())
        {
            return true;
        }
        return base.IsValidMove(deck);
    }
    protected override Task PlayCardAsync(int deck)
    {
        SaveRoot!.CardsForMarriage.Clear();
        return base.PlayCardAsync(deck);
    }
    public override async Task ContinueTrickAsync()
    {
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
        await StartNewTurnAsync(); //i think.
    }
    private int WhoWonTrick(DeckRegularDict<SixtySix2PlayerCardInformation> thisCol)
    {
        var leadCard = thisCol.First();
        var thisCard = thisCol.Last();
        if (thisCard.Suit == SaveRoot!.TrumpSuit && leadCard.Suit != SaveRoot.TrumpSuit)
        {
            return WhoTurn;
        }
        if (leadCard.Suit == SaveRoot.TrumpSuit && thisCard.Suit != SaveRoot.TrumpSuit)
        {
            return leadCard.Player;
        }
        if (thisCard.Suit == leadCard.Suit)
        {
            if (thisCard.PinochleCardValue > leadCard.PinochleCardValue)
            {
                return WhoTurn;
            }
        }
        return leadCard.Player;
    }
    private void CalculateScore(int player)
    {
        var firstPoints = SaveRoot!.CardList.Where(items => items.Player == player).Sum(items => items.Points);
        var thisPlayer = PlayerList![player];
        var secondPoints = thisPlayer.MarriageList.Sum(items => (int)items);
        if (SaveRoot.LastTrickWon == player)
        {
            firstPoints += 10;
        }
        thisPlayer.ScoreRound = firstPoints + secondPoints;
    }
    private void DrawPlayerCards()
    {
        var thisCard = _model!.Deck1!.DrawCard();
        thisCard.Drew = true;
        SingleInfo!.MainHandList.Add(thisCard);
        int newTurn;
        if (WhoTurn == 1)
        {
            newTurn = 2;
        }
        else
        {
            newTurn = 1;
        }
        var thisPlayer = PlayerList![newTurn];
        if (_model.Deck1.IsEndOfDeck())
        {
            thisCard = _model.Pile1!.GetCardInfo();
            _model.Pile1.ClearCards();
        }
        else
        {
            thisCard = _model.Deck1.DrawCard();
        }
        thisCard.Drew = true;
        thisPlayer.MainHandList.Add(thisCard);
    }
    public override async Task EndTrickAsync()
    {
        var trickList = SaveRoot!.TrickList;
        int wins = WhoWonTrick(trickList);
        SixtySix2PlayerPlayerItem thisPlayer = PlayerList![wins];
        thisPlayer.TricksWon++;
        await _aTrick!.AnimateWinAsync(wins);
        trickList.ForEach(thisCard =>
        {
            thisCard.Points = thisCard.PinochleCardValue;
            thisCard.Player = wins;
        });
        SaveRoot.CardList.AddRange(trickList);
        WhoTurn = wins;
        SingleInfo = PlayerList.GetWhoPlayer();
        if (SingleInfo.FirstMarriage != EnumMarriage.None)
            SingleInfo.MarriageList.Add(SingleInfo.FirstMarriage);
        SingleInfo.FirstMarriage = EnumMarriage.None;
        int lefts = SingleInfo.MainHandList.Count;
        if (lefts == 0)
        {
            if (_model!.Pile1!.PileEmpty() == false)
            {
                throw new CustomBasicException("Never went to end");
            }
            SaveRoot.LastTrickWon = WhoTurn;
            _toast.ShowInfoToast($"{SingleInfo.NickName} gets an extra 10 points for winning the last trick");
        }
        CalculateScore(wins);
        if (lefts == 0)
        {
            SaveRoot.BonusPoints++;
            await this.RoundOverNextAsync();
            return;
        }
        if (_model!.Pile1!.PileEmpty() == false)
        {
            DrawPlayerCards();
        }
        SingleInfo = PlayerList.GetSelf();
        _model.PlayerHand1!.EndTurn();
        SortCards();
        SingleInfo = PlayerList.GetWhoPlayer();
        await StartNewTrickAsync();
    }
    private async Task StartNewTrickAsync()
    {
        _aTrick!.ClearBoard();
        _command!.ManuelFinish = true; //because it could be somebody else's turn.
        await StartNewTurnAsync();
    }
    Task IStartNewGame.ResetAsync()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.GamePointsGame = 0;
            thisPlayer.GamePointsRound = 0;
        });
        return Task.CompletedTask;
    }
    public bool CanExchangeForDiscard()
    {
        if (_model!.Pile1!.PileEmpty() == true)
        {
            return false;
        }
        var thisCard = _model.Pile1.GetCardInfo();
        if (thisCard.Value == EnumRegularCardValueList.Nine)
        {
            return false;
        }
        return SaveRoot!.CardList.Any(items => items.Player == WhoTurn);
    }
    private int CalculateGamePoints()
    {
        if (SingleInfo!.ScoreRound < 66)
        {
            return -2; //means opponent will get them.
        }
        int otherPlayer;
        if (WhoTurn == 1)
        {
            otherPlayer = 2;
        }
        else
        {
            otherPlayer = 1;
        }
        if (!SaveRoot!.CardList.Any(items => items.Player == otherPlayer))
        {
            return 3 + SaveRoot.BonusPoints;
        }
        var thisPlayer = PlayerList![otherPlayer];
        if (thisPlayer.ScoreRound < 33)
        {
            return 2 + SaveRoot.BonusPoints;
        }
        return 1 + SaveRoot.BonusPoints;
    }
    public async Task ExchangeDiscardAsync(int deck)
    {
        var thisCard = _model!.Pile1!.GetCardInfo();
        _model.Pile1.RemoveFromPile();
        await Aggregator.AnimatePickUpDiscardAsync(thisCard);
        SingleInfo!.MainHandList.RemoveObjectByDeck(deck);
        var newCard = _gameContainer.DeckList!.GetSpecificItem(deck);
        thisCard.Drew = true;
        SingleInfo.MainHandList.Add(thisCard);
        if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
        {
            SortCards();
        }
        await AnimatePlayAsync(newCard);
        await ContinueTurnAsync();
    }
    public async Task GoOutAsync()
    {
        int points = CalculateGamePoints();
        int newTurn;
        if (WhoTurn == 1)
        {
            newTurn = 2;
        }
        else
        {
            newTurn = 1;
        }
        var thisPlayer = PlayerList![newTurn];
        if (points > 0)
        {
            SingleInfo!.GamePointsRound = points;
            SingleInfo.GamePointsGame += points;
        }
        else if (points == -2)
        {
            thisPlayer.GamePointsRound = 2;
            thisPlayer.GamePointsGame += 2;
        }
        else
        {
            SingleInfo!.GamePointsRound = 0;
            thisPlayer.GamePointsRound = 0;
        }
        if (!PlayerList.Any(items => items.GamePointsGame >= 7))
        {
            SaveRoot!.BonusPoints = 0;
            await this.RoundOverNextAsync();
            return;
        }
        SingleInfo = PlayerList.OrderByDescending(items => items.GamePointsGame).First();
        await ShowWinAsync();
    }
    public EnumMarriage WhichMarriage(IDeckDict<SixtySix2PlayerCardInformation> thisList)
    {
        if (thisList.Count != 2)
        {
            return EnumMarriage.None;
        }
        if (thisList.First().Suit != thisList.Last().Suit)
        {
            return EnumMarriage.None;//the suits must match.
        }
        if (!thisList.Any(items => items.Value == EnumRegularCardValueList.King))
        {
            return EnumMarriage.None;
        }
        if (!thisList.Any(items => items.Value == EnumRegularCardValueList.Queen))
        {
            return EnumMarriage.None;
        }
        if (thisList.First().Suit == SaveRoot!.TrumpSuit)
        {
            return EnumMarriage.Trumps;
        }
        return EnumMarriage.Regular;
    }
    public bool CanAnnounceMarriageAtBeginning => SaveRoot!.CardList.Count == 0 && _model.TrickArea1!.IsLead == false && SaveRoot.CardsForMarriage.Count == 0;
    public bool CanShowMarriage(EnumMarriage whichMarriage)
    {
        if (whichMarriage == EnumMarriage.None)
        {
            throw new CustomBasicException("If there was no marriage, this function should not be called");
        }
        SingleInfo = PlayerList!.GetWhoPlayer();
        int newValue = SingleInfo.ScoreRound + (int)whichMarriage;
        return newValue < 66;
    }
    public async Task AnnounceMarriageAsync(BasicList<int> thisList)
    {
        var newList = thisList.GetNewObjectListFromDeckList(_gameContainer.DeckList!);
        var thisMarriage = WhichMarriage(newList);
        if (thisMarriage == EnumMarriage.None)
        {
            throw new CustomBasicException("Must have been a marriage.  Otherwise; can't process");
        }
        _model!.Marriage1!.PopulateObjects(newList);
        _model.Marriage1.Visible = true;
        if (CanAnnounceMarriageAtBeginning)
        {
            SingleInfo!.FirstMarriage = thisMarriage;
        }
        else
        {
            SingleInfo!.MarriageList.Add(thisMarriage);
            CalculateScore(WhoTurn);
        }
        SaveRoot!.CardsForMarriage = thisList;
        _command.UpdateAll();
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelaySeconds(2);
        }
        _model.Marriage1.Visible = false;
        await ContinueTurnAsync();
    }
}