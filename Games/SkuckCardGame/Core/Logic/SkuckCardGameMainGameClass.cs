namespace SkuckCardGame.Core.Logic;
[SingletonGame]
public class SkuckCardGameMainGameClass
    : TrickGameClass<EnumSuitList, SkuckCardGameCardInformation, SkuckCardGamePlayerItem, SkuckCardGameSaveInfo>
        , IMiscDataNM, IStartNewGame, ISerializable
{
    private readonly SkuckCardGameVMData _model;
    private readonly CommandContainer _command;
    private readonly SkuckCardGameGameContainer _gameContainer;
    private readonly IAdvancedTrickProcesses _aTrick;
    private readonly IBidProcesses _bidProcesses;
    private readonly IPlayChoiceProcesses _choiceProcesses;
    private readonly ITrumpProcesses _trumpProcesses;
    public SkuckCardGameMainGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        SkuckCardGameVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<SkuckCardGameCardInformation> cardInfo,
        CommandContainer command,
        SkuckCardGameGameContainer gameContainer,
        ITrickData trickData,
        ITrickPlay trickPlay,
        IAdvancedTrickProcesses aTrick,
        IBidProcesses bidProcesses,
        IPlayChoiceProcesses choiceProcesses,
        ITrumpProcesses trumpProcesses,
        ISystemError error,
        IToast toast
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, trickData, trickPlay, error, toast)
    {
        _model = currentMod;
        _command = command;
        _gameContainer = gameContainer;
        _aTrick = aTrick;
        _bidProcesses = bidProcesses;
        _choiceProcesses = choiceProcesses;
        _trumpProcesses = trumpProcesses;
        _gameContainer.ComputerTurnAsync = ComputerTurnAsync;
        _gameContainer.StartNewTrickAsync = StartNewTrickAsync;
        _gameContainer.ShowHumanCanPlayAsync = ShowHumanCanPlayAsync;
    }
    public override Task PopulateSaveRootAsync()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.SavedTemp = thisPlayer!.TempHand!.CardList.ToRegularDeckDict();
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
                thisPlayer.TempHand.Game = EnumPlayerBoardGameList.Skuck;
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
        _model.Bid1!.UnselectAll();
        _model.Suit1!.UnselectAll();
        PlayerList!.ForEach(thisPlayer =>
        {
            if (thisPlayer.PlayerCategory == EnumPlayerCategory.Self)
            {
                thisPlayer.BidVisible = true;
            }
            else
            {
                thisPlayer.BidVisible = false;
            }
        });
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
    protected override Task LastPartOfSetUpBeforeBindingsAsync()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            var thisList = _model!.Deck1!.DrawSeveralCards(16);
            thisPlayer.TempHand!.ClearBoard(thisList);
        });
        EvalulateStrength();
        WhoTurn = WhoChoosesTrump();
        return base.LastPartOfSetUpBeforeBindingsAsync();
    }
    protected override void LoadVM()
    {
        base.LoadVM();
        SaveRoot!.LoadMod(_model!);
    }
    protected override async Task ComputerTurnAsync()
    {
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelaySeconds(.75);
        }
        if (SaveRoot!.WhatStatus == EnumStatusList.NormalPlay)
        {
            var moveList = SingleInfo!.MainHandList.Where(Items => IsValidMove(Items.Deck)).Select(Items => Items.Deck).ToBasicList();
            var otherList = SingleInfo.TempHand!.ValidCardList;
            otherList.KeepConditionalItems(Items => IsValidMove(Items.Deck));
            var finList = otherList.Select(Items => Items.Deck).ToBasicList();
            moveList.AddRange(finList);
            if (moveList.Count == 0)
            {
                throw new CustomBasicException("There must be at least one move for the computer");
            }
            await PlayCardAsync(moveList.GetRandomItem());
            return;
        }
        if (SaveRoot.WhatStatus == EnumStatusList.ChooseBid)
        {
            _model!.BidAmount = _model.Bid1!.NumberToChoose();
            await _bidProcesses.ProcessBidAmountAsync();
            return;
        }
        if (SaveRoot.WhatStatus == EnumStatusList.ChooseTrump)
        {
            SaveRoot.TrumpSuit = _model!.Suit1!.ItemToChoose();
            await _trumpProcesses.TrumpChosenAsync();
            return;
        }
        int ask1 = _gameContainer.Random.GetRandomNumber(2);
        if (ask1 == 1)
        {
            await _choiceProcesses.ChooseToPlayAsync();
        }
        else
        {
            await _choiceProcesses.ChooseToPassAsync();
        }
    }
    protected override async Task StartSetUpAsync(bool isBeginning)
    {
        LoadControls();
        PlayerList!.ForEach(thisPlayer => thisPlayer.SavedTemp.Clear());
        LoadPlayerControls();
        LoadVM();
        SaveRoot!.WhatStatus = EnumStatusList.ChooseTrump;
        SaveRoot.RoundNumber++;
        PlayerList.ForEach(thisPlayer =>
        {
            thisPlayer.BidAmount = 0;
            thisPlayer.BidVisible = false;
            thisPlayer.TricksWon = 0;
            thisPlayer.StrengthHand = 0;
            thisPlayer.TieBreaker = "0";
        });
        SaveRoot.TrumpSuit = EnumSuitList.None;
        _model.BidAmount = -1;
        _model.Bid1!.UnselectAll();
        _model.Suit1!.UnselectAll();
        await base.StartSetUpAsync(isBeginning);
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "choosetoplay":
                await _choiceProcesses.ChooseToPlayAsync();
                break;
            case "choosetopass":
                await _choiceProcesses.ChooseToPassAsync();
                break;
            case "trump":
                SaveRoot!.TrumpSuit = await js.DeserializeObjectAsync<EnumSuitList>(content);
                await _trumpProcesses.TrumpChosenAsync();
                break;
            case "bid":
                //int plays = PlayerList.Where(items => items.PlayerCategory == EnumPlayerCategory.OtherHuman).Single().Id;
                _model!.BidAmount = int.Parse(content);
                await _bidProcesses.ProcessBidAmountAsync();
                break;
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
        SingleInfo = PlayerList!.GetWhoPlayer();
        SingleInfo.MainHandList.UnhighlightObjects();
        _command.ManuelFinish = true; //because it could be somebody else's turn.
        WhoTurn = await PlayerList.CalculateWhoTurnAsync();
        this.ShowTurn();
        await SaveStateAsync();
        await ContinueTurnAsync();
    }
    private int WhoWonTrick(DeckRegularDict<SkuckCardGameCardInformation> thisCol)
    {
        if (thisCol.Count != 2)
        {
            throw new CustomBasicException("The trick list must have 2 cards");
        }
        var leadCard = thisCol.First();
        int begins = leadCard.Player;
        var otherCard = thisCol.Last();
        if (otherCard.Suit == SaveRoot!.TrumpSuit && leadCard.Suit != SaveRoot.TrumpSuit && CanConsiderTrump(otherCard) == true)
        {
            return WhoTurn;
        }
        if (leadCard.Suit == SaveRoot.TrumpSuit && otherCard.Suit != SaveRoot.TrumpSuit && CanConsiderTrump(otherCard) == true)
        {
            return begins;
        }
        if (leadCard.Suit == otherCard.Suit)
        {
            if (otherCard.Value > leadCard.Value)
            {
                return WhoTurn;
            }
        }
        return begins;
    }
    public override async Task EndTrickAsync()
    {
        var trickList = SaveRoot!.TrickList;
        int wins = WhoWonTrick(trickList);
        SkuckCardGamePlayerItem thisPlayer = PlayerList![wins];
        thisPlayer.TricksWon += HowManyWins(trickList);
        await _aTrick!.AnimateWinAsync(wins);
        //has to get self this time.
        SingleInfo = PlayerList.GetSelf();
        if (SingleInfo.MainHandList.Count == 0 && SingleInfo.TempHand!.IsFinished == true)
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
            thisPlayer.PerfectRounds = 0;
        });
        SaveRoot!.RoundNumber = 0;
        return Task.CompletedTask;
    }
    public override async Task ContinueTurnAsync()
    {
        if (SaveRoot!.WhatStatus != EnumStatusList.ChooseBid)
        {
            await base.ContinueTurnAsync();
            PlayerList!.ForEach(thisItem => thisItem.TempHand!.ReportCanExecuteChange()); //just in case it got hosed.
            return;
        }
        //SingleInfo = PlayerList!.GetSelf();
        //WhoTurn = SingleInfo.Id;
        //this.ShowTurn();
        //if (SingleInfo.BidAmount > 0)
        //{
        //    if (BasicData!.MultiPlayer == false)
        //    {
        //        throw new CustomBasicException("If you already bidded, the computer player should have bidded then moved on");
        //    }
        //    await SaveStateAsync();
        //    Network!.IsEnabled = true;
        //    return; //to wait for the other player.
        //}
        await base.ContinueTurnAsync();
    }
    private void EvalulateStrength()
    {
        DeckRegularDict<SkuckCardGameCardInformation> firstCollection = new();
        DeckRegularDict<SkuckCardGameCardInformation> secondCollection = new();
        DeckRegularDict<SkuckCardGameCardInformation> tempList;
        PlayerList!.ForEach(tempPlayer =>
        {
            tempPlayer.MainHandList.ForEach(thisCard =>
            {
                if (tempPlayer.Id == 1)
                {
                    firstCollection.Add(thisCard);
                }
                else
                {
                    secondCollection.Add(thisCard);
                }
            });
            tempList = tempPlayer.TempHand!.KnownList;
            tempList.ForEach(ThisCard =>
            {
                if (tempPlayer.Id == 1)
                {
                    firstCollection.Add(ThisCard);
                }
                else
                {
                    secondCollection.Add(ThisCard);
                }
            });
        });
        if (firstCollection.Count != 18 || secondCollection.Count != 18)
        {
            throw new CustomBasicException("Both players has 18 cards to begin with to evaluate strength hand");
        }
        int firstPoints = 0;
        int secondPoints = 0;
        firstCollection.ForEach(thisCard =>
        {
            if (thisCard.Value == EnumRegularCardValueList.HighAce)
            {
                firstPoints += 5;
            }
            else if (thisCard.Value == EnumRegularCardValueList.King)
            {
                firstPoints += 4;
            }
            else if (thisCard.Value == EnumRegularCardValueList.Queen)
            {
                firstPoints += 3;
            }
            else if (thisCard.Value == EnumRegularCardValueList.Jack)
            {
                firstPoints += 2;
            }
            else if (thisCard.Value == EnumRegularCardValueList.Ten)
            {
                firstPoints += 1;
            }
        });
        secondCollection.ForEach(thisCard =>
        {
            if (thisCard.Value == EnumRegularCardValueList.HighAce)
            {
                secondPoints += 5;
            }
            else if (thisCard.Value == EnumRegularCardValueList.King)
            {
                secondPoints += 4;
            }
            else if (thisCard.Value == EnumRegularCardValueList.Queen)
            {
                secondPoints += 3;
            }
            else if (thisCard.Value == EnumRegularCardValueList.Jack)
            {
                secondPoints += 2;
            }
            else if (thisCard.Value == EnumRegularCardValueList.Ten)
            {
                secondPoints += 1;
            }
        });
        PlayerList.First().StrengthHand = firstPoints;
        PlayerList.Last().StrengthHand = secondPoints;
        if (firstPoints != secondPoints)
        {
            PlayerList.ForEach(ThisPlayer => ThisPlayer.TieBreaker = "");
            return;
        }
        int firstNumber;
        int secondNumber;
        string whatText;
        int whos;
        for (int x = 2; x <= 14; x++)
        {
            firstNumber = firstCollection.Count(Items => Items.Value == x.ToEnum<EnumRegularCardValueList>());
            secondNumber = secondCollection.Count(Items => Items.Value == x.ToEnum<EnumRegularCardValueList>());
            if (firstNumber != secondNumber)
            {
                if (firstNumber > secondNumber)
                {
                    whos = 1;
                }
                else
                {
                    whos = 2;
                }
                if (whos == 1)
                {
                    whatText = $"{x},y";
                }
                else
                {
                    whatText = $"{x},n";
                }
                PlayerList.First().TieBreaker = whatText;
                if (whos == 1)
                {
                    whatText = $"{x},n";
                }
                else
                {
                    whatText = $"{x},y";
                }
                PlayerList.Last().TieBreaker = whatText;
            }
        }
    }
    private int WhoChoosesTrump()
    {
        if (_gameContainer.Test.DoubleCheck)
        {
            return PlayerList.GetSelf().Id; //both shows it for testing.
        }
        int firstNumber;
        int secondNumber;
        firstNumber = PlayerList.First().StrengthHand;
        secondNumber = PlayerList.Last().StrengthHand;
        if (firstNumber < secondNumber)
        {
            return 1;
        }
        else if (secondNumber > firstNumber)
        {
            return 2;
        }
        string text = PlayerList.First().TieBreaker;
        if (text.Contains('y'))
        {
            return 1;
        }
        return 2;
    }
    private async Task GameOverAsync()
    {
        int wins = WhoWonGame();
        if (wins == 0)
        {
            await ShowTieAsync();
        }
        else
        {
            SingleInfo = PlayerList![wins];
            await ShowWinAsync();
        }
    }
    private int WhoWonGame()
    {
        if (PlayerList.Count != 2)
        {
            throw new CustomBasicException("2 Player Game Only.  Rethink");
        }
        var firstPlayer = PlayerList.First();
        var secondPlayer = PlayerList.Last();
        if (firstPlayer.TotalScore > secondPlayer.TotalScore)
        {
            return 1;
        }
        if (secondPlayer.TotalScore > firstPlayer.TotalScore)
        {
            return 2;
        }
        if (firstPlayer.PerfectRounds == secondPlayer.PerfectRounds)
        {
            return 0;
        }
        if (firstPlayer.PerfectRounds > secondPlayer.PerfectRounds)
        {
            return 1;
        }
        return 2;
    }
    private bool CanConsiderTrump(SkuckCardGameCardInformation thisCard)
    {
        if (SaveRoot!.TrumpSuit != EnumSuitList.Clubs)
        {
            return true;
        }
        if (thisCard.Suit != EnumSuitList.Clubs)
        {
            return true;
        }
        if (thisCard.Value == EnumRegularCardValueList.Jack)
        {
            return false;
        }
        return true;
    }
    private static int HowManyWins(IDeckDict<SkuckCardGameCardInformation> trickList)
    {
        if (trickList.Any(items => items.Suit == EnumSuitList.Clubs && items.Value == EnumRegularCardValueList.Jack))
        {
            return 2;
        }
        return 1;
    }
    private void CalculateScore()
    {
        int nums;
        PlayerList!.ForEach(thisPlayer =>
        {
            nums = thisPlayer.TricksWon - thisPlayer.BidAmount;
            if (nums == 0)
            {
                thisPlayer.PerfectRounds++;
            }
            else if (nums > 0)
            {
                nums *= -1;
            }
            thisPlayer.TotalScore += nums;
        });
    }
    public override async Task EndRoundAsync()
    {
        CalculateScore();
        PlayerList!.ForEach(thisPlayer => thisPlayer.BidVisible = true);
        if (SaveRoot!.RoundNumber == 4)
        {
            await GameOverAsync();
            return;
        }
        await this.RoundOverNextAsync();
    }
}