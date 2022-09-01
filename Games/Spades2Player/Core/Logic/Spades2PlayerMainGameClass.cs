namespace Spades2Player.Core.Logic;
[SingletonGame]
public class Spades2PlayerMainGameClass
    : TrickGameClass<EnumSuitList, Spades2PlayerCardInformation, Spades2PlayerPlayerItem, Spades2PlayerSaveInfo>
        , IMiscDataNM, IStartNewGame, ISerializable
{
    private readonly Spades2PlayerVMData _model;
    private readonly CommandContainer _command;
    private readonly Spades2PlayerGameContainer _gameContainer;
    private readonly IAdvancedTrickProcesses _aTrick;
    public Spades2PlayerMainGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        Spades2PlayerVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<Spades2PlayerCardInformation> cardInfo,
        CommandContainer command,
        Spades2PlayerGameContainer gameContainer,
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
    }
    private bool _mustKeep;
    public override async Task FinishGetSavedAsync()
    {
        LoadControls();
        LoadVM();
        if (SaveRoot!.GameStatus == EnumGameStatus.ChooseCards)
        {
            _model.OtherPile!.SavedDiscardPiles(SaveRoot.OtherPile!);
        }
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
    protected override void LinkHand()
    {
        SingleInfo = PlayerList!.GetSelf();
        _model!.PlayerHand1!.HandList = SingleInfo.MainHandList;
        PrepSort();
    }
    protected override void LoadVM()
    {
        base.LoadVM();
        SaveRoot!.LoadMod(_model!);
    }
    protected override Task LastPartOfSetUpBeforeBindingsAsync()
    {
        PrepStartTurn();
        return base.LastPartOfSetUpBeforeBindingsAsync();
    }
    public override async Task DiscardAsync(Spades2PlayerCardInformation thisCard)
    {
        _model.OtherPile!.ClearCards();
        thisCard.Drew = false;
        await AnimatePlayAsync(thisCard);
        if (SaveRoot!.FirstCard == true)
        {
            _mustKeep = true;
            SaveRoot.FirstCard = false;
            await DrawAsync();
            return;
        }
    }
    protected override bool CardToCurrentPile()
    {
        return SaveRoot!.FirstCard;
    }
    protected override async Task AfterDrawingAsync()
    {
        if (SaveRoot!.FirstCard == true)
        {
            await ContinueTurnAsync();
        }
        else
        {
            await EndTurnAsync();
        }
    }
    protected override async Task ComputerTurnAsync()
    {
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelaySeconds(.75);
        }
        if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Computer)
        {
            throw new CustomBasicException("Must be computer player to begin with");
        }
        if (SaveRoot!.GameStatus == EnumGameStatus.ChooseCards)
        {
            int ask1 = _gameContainer.Random.GetRandomNumber(2);
            if (ask1 == 1)
            {
                await AcceptCardAsync();
                return;
            }
            await DiscardAsync(_model.OtherPile!.GetCardInfo());
            return;
        }
        if (SaveRoot.GameStatus == EnumGameStatus.Bidding)
        {
            _model!.BidAmount = _model!.Bid1!.NumberToChoose();
            await ProcessBidAsync();
            return;
        }
        var moveList = SingleInfo.MainHandList.Where(xx => IsValidMove(xx.Deck)).Select(xx => xx.Deck).ToBasicList();
        await PlayCardAsync(moveList.GetRandomItem());
    }
    protected override async Task AddCardAsync(Spades2PlayerCardInformation thisCard, Spades2PlayerPlayerItem tempPlayer)
    {
        if (_mustKeep == true)
        {
            await base.AddCardAsync(thisCard, tempPlayer);
        }
        else
        {
            await DiscardAsync(thisCard); //it will draw but since you can't keep will discard it.
        }
    }
    public override async Task ContinueTurnAsync()
    {
        if (SaveRoot!.NeedsToDraw == true)
        {
            SaveRoot.NeedsToDraw = false;
            await DrawAsync();
            return;
        }
        if (SaveRoot.GameStatus != EnumGameStatus.ChooseCards && SingleInfo!.MainHandList.Count == 0)
        {
            throw new CustomBasicException("If you are not choosing cards, you must have at least one card");
        }
        if (SaveRoot.GameStatus == EnumGameStatus.ChooseCards && SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
        {
            _model.OtherPile!.Visible = true;
        }
        await base.ContinueTurnAsync();
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        SaveRoot!.RoundNumber++;
        SaveRoot.TrickStatus = EnumTrickStatus.FirstTrick;
        SaveRoot.GameStatus = EnumGameStatus.ChooseCards;
        LoadControls();
        LoadVM();
        SaveRoot.NeedsToDraw = true;
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.TricksWon = 0;
            thisPlayer.HowManyBids = -1;
        });
        return base.StartSetUpAsync(isBeginning);
    }
    protected override void PrepStartTurn()
    {
        base.PrepStartTurn();
        _model!.BidAmount = -1;
        _model.Bid1!.UnselectAll();
        if (SaveRoot!.GameStatus == EnumGameStatus.ChooseCards)
        {
            if (_model.Deck1!.IsEndOfDeck() == true)
            {
                SaveRoot.GameStatus = EnumGameStatus.Bidding;
            }
            else
            {
                SaveRoot.FirstCard = true;
            }
        }
    }
    public override async Task PopulateSaveRootAsync()
    {
        if (SaveRoot!.GameStatus == EnumGameStatus.ChooseCards)
        {
            SaveRoot.OtherPile = _model.OtherPile!.GetSavedPile();
        }
        await base.PopulateSaveRootAsync();
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "acceptcard":
                await AcceptCardAsync();
                break;
            case "bid":
                _model!.BidAmount = int.Parse(content);
                await ProcessBidAsync();
                break;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override async Task StartNewTurnAsync()
    {
        await base.StartNewTurnAsync();
        if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
        {
            _model!.PlayerHand1!.EndTurn();
        }
        if (_model!.Deck1!.IsEndOfDeck() == false && SaveRoot!.GameStatus != EnumGameStatus.ChooseCards)
        {
            throw new CustomBasicException("If its not the end of the deck, then status must be choose cards");
        }
        if (_model.Deck1.IsEndOfDeck() == true && SaveRoot!.GameStatus == EnumGameStatus.ChooseCards)
        {
            throw new CustomBasicException("If its the end of the deck, gamestatus cannot be choosecards");
        }
        if (SaveRoot!.GameStatus == EnumGameStatus.ChooseCards)
        {
            SaveRoot.NeedsToDraw = true;
        }
        await ContinueTurnAsync();
    }
    public override async Task EndTurnAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        SingleInfo.MainHandList.UnhighlightObjects();
        _command.ManuelFinish = true; //because it could be somebody else's turn.
        WhoTurn = await PlayerList.CalculateWhoTurnAsync();
        await StartNewTurnAsync();
    }
    private int WhoWonTrick(DeckRegularDict<Spades2PlayerCardInformation> thisCol)
    {
        var leadCard = thisCol.First();
        var thisCard = thisCol.Last();
        if (thisCard.Suit == EnumSuitList.Spades && leadCard.Suit != EnumSuitList.Spades)
        {
            return WhoTurn;
        }
        if (leadCard.Suit == EnumSuitList.Spades && thisCard.Suit != EnumSuitList.Spades)
        {
            return leadCard.Player;
        }
        if (thisCard.Suit == leadCard.Suit)
        {
            if (thisCard.Value > leadCard.Value)
            {
                return WhoTurn;
            }
        }
        return leadCard.Player;
    }
    public override async Task EndTrickAsync()
    {
        var trickList = SaveRoot!.TrickList;
        if (trickList.Any(Items => Items.Suit == EnumSuitList.Spades))
        {
            SaveRoot.TrickStatus = EnumTrickStatus.SuitBroken;
        }
        else if (SaveRoot.TrickStatus == EnumTrickStatus.FirstTrick)
        {
            SaveRoot.TrickStatus = EnumTrickStatus.NoSuit;
        }
        int wins = WhoWonTrick(trickList);
        Spades2PlayerPlayerItem player = PlayerList![wins];
        player.TricksWon++;
        await _aTrick!.AnimateWinAsync(wins);
        if (SingleInfo!.MainHandList.Count == 0)
        {
            await EndRoundAsync();
            return;
        }
        _model!.PlayerHand1!.EndTurn();
        WhoTurn = wins;
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
            thisPlayer.TotalScore = 0;
            thisPlayer.Bags = 0;
            thisPlayer.CurrentScore = 0;
        });
        SaveRoot!.RoundNumber = 0;
        return Task.CompletedTask;
    }
    private void GetWinningPlayer()
    {
        SingleInfo = PlayerList.OrderByDescending(Items => Items.TotalScore).Take(1).Single();
    }
    private bool CanEndGame()
    {
        if (SaveRoot!.RoundNumber >= 12)
        {
            GetWinningPlayer();
            return true;
        }
        if (PlayerList.Any(items => items.TotalScore >= 500))
        {
            GetWinningPlayer();
            return true;
        }
        return false;
    }
    public override async Task EndRoundAsync()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.CalculateScore();
        });
        if (CanEndGame() == true)
        {
            await ShowWinAsync();
            return;
        }
        await this.RoundOverNextAsync();
    }
    public async Task AcceptCardAsync()
    {
        SaveRoot!.FirstCard = false;
        _mustKeep = false;
        if (_model.OtherPile!.PileEmpty() == true)
        {
            throw new CustomBasicException("Nothing in other pile.  Rethink");
        }
        var thisCard = _model.OtherPile.GetCardInfo();
        SingleInfo!.MainHandList.Add(thisCard);
        _model.OtherPile.Visible = false;
        _command.UpdateAll();
        if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
        {
            thisCard.Drew = true;
            SortCards();
        }
        await DrawAsync();
    }
    public async Task ProcessBidAsync()
    {
        if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
        {
            _model!.Bid1!.SelectNumberValue(_model.BidAmount);
            _command.UpdateAll();
            if (Test!.NoAnimations == false)
            {
                await Delay!.DelaySeconds(.5);
            }
        }
        SingleInfo.HowManyBids = _model!.BidAmount;
        if (PlayerList.All(Items => Items.HowManyBids > -1))
        {
            SaveRoot!.GameStatus = EnumGameStatus.Normal;
            _aTrick!.ClearBoard();
        }
        await EndTurnAsync();
    }
}