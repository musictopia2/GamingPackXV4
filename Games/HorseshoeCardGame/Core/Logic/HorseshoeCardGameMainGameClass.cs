namespace HorseshoeCardGame.Core.Logic;
[SingletonGame]
public class HorseshoeCardGameMainGameClass
    : TrickGameClass<EnumSuitList, HorseshoeCardGameCardInformation, HorseshoeCardGamePlayerItem, HorseshoeCardGameSaveInfo>
        , IStartNewGame, ISerializable
{
    private readonly HorseshoeCardGameVMData _model;
    private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
    private readonly HorseshoeCardGameGameContainer _gameContainer; //if we don't need it, take it out.
    private readonly IAdvancedTrickProcesses _aTrick;
    public HorseshoeCardGameMainGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        HorseshoeCardGameVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<HorseshoeCardGameCardInformation> cardInfo,
        CommandContainer command,
        HorseshoeCardGameGameContainer gameContainer,
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
    public override Task PopulateSaveRootAsync()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.SavedTemp = thisPlayer.TempHand!.CardList.ToRegularDeckDict();
        });
        return base.PopulateSaveRootAsync();
    }
    private void LoadPlayerControls()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            if (thisPlayer.TempHand == null)
            {
                thisPlayer.TempHand = new(_command);
                thisPlayer.TempHand.Game = EnumPlayerBoardGameList.HorseShoe;
                thisPlayer.TempHand.IsSelf = thisPlayer.PlayerCategory == EnumPlayerCategory.Self;
            }
            if (thisPlayer.SavedTemp.Count != 0)
            {
                thisPlayer.TempHand.CardList.ReplaceRange(thisPlayer.SavedTemp);
            }
        });
    }
    public override async Task FinishGetSavedAsync()
    {
        LoadControls();
        LoadVM();
        LoadPlayerControls();
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
    protected override async Task ShowHumanCanPlayAsync()
    {
        await base.ShowHumanCanPlayAsync();
        PlayerList.ForEach(x => x.TempHand!.ReportCanExecuteChange());
        _model.TrickArea1.ReportCanExecuteChange();
    }
    protected override async Task ComputerTurnAsync()
    {
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelaySeconds(.5);
        }
        var moveList = SingleInfo!.MainHandList.Where(items => IsValidMove(items.Deck)).Select(items => items.Deck).ToBasicList();
        var otherList = SingleInfo.TempHand!.ValidCardList;
        otherList.KeepConditionalItems(items => IsValidMove(items.Deck));
        var finList = otherList.Select(items => items.Deck).ToBasicList();
        moveList.AddRange(finList);
        if (moveList.Count == 0)
        {
            throw new CustomBasicException("There must be at least one move for the computer");
        }
        await PlayCardAsync(moveList.GetRandomItem());
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        LoadControls();
        PlayerList!.ForEach(thisPlayer => thisPlayer.SavedTemp.Clear());
        LoadPlayerControls();
        LoadVM();
        SaveRoot!.FirstCardPlayed = false;
        PlayerList.ForEach(thisPlayer => thisPlayer.TricksWon = 0);
        return base.StartSetUpAsync(isBeginning);
    }
    protected override Task LastPartOfSetUpBeforeBindingsAsync()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            var thisList = _model!.Deck1!.DrawSeveralCards(8);
            thisPlayer.TempHand!.ClearBoard(thisList);
        });
        _model.TrickArea1!.ClearBoard();
        return base.LastPartOfSetUpBeforeBindingsAsync();
    }

    public override async Task StartNewTurnAsync()
    {
        await base.StartNewTurnAsync();
        await ContinueTurnAsync();
    }
    public override async Task EndTurnAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        SingleInfo.MainHandList.UnhighlightObjects();
        if (_model.TrickArea1!.DidPlay2Cards == true)
        {
            SaveRoot!.FirstCardPlayed = true;
            await ContinueTurnAsync();
            return;
        }
        _command.ManuelFinish = true; //because it could be somebody else's turn.
        WhoTurn = await PlayerList.CalculateWhoTurnAsync();
        this.ShowTurn();
        await SaveStateAsync();
        await ContinueTurnAsync();
    }
    private static int WhoWonTrick(DeckRegularDict<HorseshoeCardGameCardInformation> thisCol)
    {
        if (thisCol.Count != 4)
        {
            throw new CustomBasicException("Must have 4 cards for the trick list to see who won");
        }
        var thisCard = thisCol.First();
        int begins = thisCard.Player;
        EnumRegularCardValueList nums = thisCard.Value;
        if (thisCol.All(Items => Items.Value == nums))
        {
            return begins;
        }
        DeckRegularDict<HorseshoeCardGameCardInformation> playerStarted = new();
        DeckRegularDict<HorseshoeCardGameCardInformation> otherPlayer = new();
        playerStarted.Add(thisCol.First());
        playerStarted.Add(thisCol.Last());
        otherPlayer.Add(thisCol[1]);
        otherPlayer.Add(thisCol[2]);
        HorseshoeCardGameCardInformation firstCard = playerStarted.First();
        HorseshoeCardGameCardInformation secondCard = playerStarted.Last();
        EnumSuitList whichSuit = firstCard.Suit;
        EnumRegularCardValueList pairAmount;
        EnumRegularCardValueList highestSuitNumber = EnumRegularCardValueList.None;
        int possibleWinPlayer = 0;
        if (firstCard.Value == secondCard.Value)
        {
            pairAmount = firstCard.Value;
        }
        else
        {
            pairAmount = EnumRegularCardValueList.None;
            var card = thisCol.Where(x => x.Suit == whichSuit).OrderByDescending(x => x.Value).Take(1).Single();
            highestSuitNumber = card.Value;
            possibleWinPlayer = card.Player;
        }
        firstCard = otherPlayer.First();
        secondCard = otherPlayer.Last();
        if (firstCard.Value != secondCard.Value && pairAmount.Value > 0)
        {
            return begins;
        }
        if (firstCard.Value == secondCard.Value)
        {
            if (firstCard.Value > pairAmount)
            {
                if (begins == 1)
                {
                    return 2;
                }
                return 1;
            }
        }
        if (pairAmount.Value > 0)
        {
            return begins;
        }
        if (possibleWinPlayer == 0)
        {
            throw new CustomBasicException("Sombody had to win it");
        }
        return possibleWinPlayer;
    }
    public override bool IsValidMove(int deck)
    {
        if (deck == 0)
        {
            throw new CustomBasicException("Deck cannot be 0 for isvalidmove");
        }
        var thisList = SaveRoot!.TrickList;
        if (thisList.Count == 0 || thisList.Count == 2)
        {
            return true;
        }
        HorseshoeCardGameCardInformation firstCard;
        if (thisList.Count == 1)
        {
            firstCard = thisList.First();
        }
        else
        {
            firstCard = thisList[2];
        }
        HorseshoeCardGameCardInformation cardPlayed = _gameContainer.DeckList!.GetSpecificItem(deck);
        if (cardPlayed.Suit == firstCard.Suit)
        {
            return true;
        }
        if (_gameContainer.GetCurrentHandList == null)
        {
            throw new CustomBasicException("Nobody is getting currenthandlist.  Rethink");
        }
        var fullList = _gameContainer!.GetCurrentHandList.Invoke();
        if (fullList.Count > 14)
        {
            throw new CustomBasicException("The temp list cannot be more than 14.  Find out what happened");
        }
        return !(fullList.Any(Items => Items.Suit == firstCard.Suit));
    }
    public override async Task EndTrickAsync()
    {
        var trickList = SaveRoot!.TrickList;
        int wins = WhoWonTrick(trickList);
        HorseshoeCardGamePlayerItem thisPlayer = PlayerList![wins];
        thisPlayer.TricksWon++;
        await _aTrick!.AnimateWinAsync(wins);
        if (SingleInfo!.MainHandList.Count == 0 && SingleInfo.TempHand!.IsFinished == true)
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
        SaveRoot!.FirstCardPlayed = false;
        await StartNewTurnAsync();
    }
    Task IStartNewGame.ResetAsync()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.TotalScore = 0;
            thisPlayer.PreviousScore = 0;
        });
        return Task.CompletedTask;
    }
    private bool CanEndGame()
    {
        HorseshoeCardGamePlayerItem tempPlayer = PlayerList.Where(items => items.TotalScore >= 11).OrderByDescending(Items => Items.TotalScore).FirstOrDefault()!;
        if (tempPlayer == null)
        {
            return false;
        }
        SingleInfo = tempPlayer;
        return true;
    }
    private int MostTricks()
    {
        if (PlayerList.First().TricksWon > PlayerList.Last().TricksWon)
        {
            return 1;
        }
        return 2;
    }
    private void CalculateScore()
    {
        int most = MostTricks();
        HorseshoeCardGamePlayerItem thisPlayer = PlayerList![most];
        int wons = thisPlayer.TricksWon;
        int points;
        if (wons == 4)
        {
            points = 2;
        }
        else if (wons == 5)
        {
            points = 3;
        }
        else if (wons == 6)
        {
            points = 5;
        }
        else if (wons == 7)
        {
            points = 7;
        }
        else
        {
            throw new CustomBasicException($"Sorry, no points for {wons}");
        }
        thisPlayer.PreviousScore = points;
        thisPlayer.TotalScore += points;
        if (most == 1)
        {
            most = 2;
        }
        else
        {
            most = 1;
        }
        PlayerList[most].PreviousScore = 0;
    }
    public override async Task EndRoundAsync()
    {
        CalculateScore();
        if (CanEndGame() == true)
        {
            await ShowWinAsync();
            return;
        }
        await this.RoundOverNextAsync();
    }
}