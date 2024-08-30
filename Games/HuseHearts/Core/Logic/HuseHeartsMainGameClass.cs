namespace HuseHearts.Core.Logic;
[SingletonGame]
public class HuseHeartsMainGameClass
    : TrickGameClass<EnumSuitList, HuseHeartsCardInformation, HuseHeartsPlayerItem, HuseHeartsSaveInfo>
        , IMiscDataNM, IStartNewGame, ISerializable
{
    private readonly HuseHeartsVMData _model;
    private readonly CommandContainer _command;
    private readonly HuseHeartsGameContainer _gameContainer;
    private readonly IAdvancedTrickProcesses _aTrick;
    private readonly IToast _toast;
    private readonly IMessageBox _message;

#pragma warning disable IDE0290 // Use primary constructor
    public HuseHeartsMainGameClass(IGamePackageResolver mainContainer,
#pragma warning restore IDE0290 // Use primary constructor
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        HuseHeartsVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<HuseHeartsCardInformation> cardInfo,
        CommandContainer command,
        HuseHeartsGameContainer gameContainer,
        ITrickData trickData,
        ITrickPlay trickPlay,
        IAdvancedTrickProcesses aTrick,
        ISystemError error,
        IToast toast,
        IMessageBox message
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, trickData, trickPlay, error, toast)
    {
        _model = currentMod;
        _command = command;
        _gameContainer = gameContainer;
        _aTrick = aTrick;
        _toast = toast;
        _message = message;
    }
    public override async Task FinishGetSavedAsync()
    {
        LoadControls();
        LoadVM();
        await base.FinishGetSavedAsync();
        _aTrick!.LoadGame();
        FinalLoading();
    }
    private void FinalLoading()
    {
        _model!.Dummy1!.HandList.ReplaceRange(SaveRoot!.DummyList);
        _model.Blind1!.HandList.ReplaceRange(SaveRoot.BlindList);
        if (_model.Blind1.HandList.Count != 4)
        {
            throw new CustomBasicException("Blind must have 4 cards for finalloading");
        }
        if (SaveRoot.GameStatus == EnumStatus.ShootMoon)
        {
            _model.Blind1.Visible = false;
            return;
        }
        if (SaveRoot.WhoWinsBlind > 0)
        {
            var tempPlayer = PlayerList![SaveRoot.WhoWinsBlind];
            if (tempPlayer.PlayerCategory != EnumPlayerCategory.Self)
            {
                _model.Blind1.Visible = false;
                return;
            }
        }
        _model.Blind1.Visible = true;
    }
    private void LoadControls()
    {
        if (IsLoaded == true)
        {
            return;
        }
        IsLoaded = true;
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
        if (SaveRoot!.GameStatus == EnumStatus.ShootMoon)
        {
            throw new CustomBasicException("I doubt the computer would ever shoot the moon.  If so; rethinking is required");
        }
        if (SaveRoot.GameStatus == EnumStatus.Passing)
        {
            await ComputerPassCardsAsync();
            return;
            //throw new CustomBasicException("The computer should have already passed the cards");
        }
        BasicList<int> moveList;
        if (_model.TrickArea1!.FromDummy == false)
        {
            moveList = SingleInfo!.MainHandList.Where(x => IsValidMove(x.Deck)).Select(x => x.Deck).ToBasicList();
        }
        else
        {
            moveList = _model!.Dummy1!.HandList.Where(x => IsValidMove(x.Deck)).Select(x => x.Deck).ToBasicList();
        }
        await PlayCardAsync(moveList.First());
    }
    protected override bool CanEndTurnToContinueTrick
    {
        get
        {
            if (_model.TrickArea1!.FromDummy == true)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        LoadControls();
        LoadVM();
        SaveRoot!.RoundNumber++;
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.CurrentScore = 0;
            thisPlayer.TricksWon = 0;
            thisPlayer.HadPoints = false;
        });
        SaveRoot.TrickStatus = EnumTrickStatus.FirstTrick;
        SaveRoot.GameStatus = EnumStatus.Passing;
        SaveRoot.WhoWinsBlind = 0;
        SaveRoot.WhoLeadsTrick = 0; //hopefully the standard whostarts is okay (?)
        return base.StartSetUpAsync(isBeginning);
    }
    protected override Task LastPartOfSetUpBeforeBindingsAsync()
    {
        SaveRoot!.BlindList = _model!.Deck1!.DrawSeveralCards(4);
        SaveRoot.BlindList.ForEach(ThisCard => ThisCard.IsUnknown = true); //this is very important too obviously.
        FinalLoading();
        return base.LastPartOfSetUpBeforeBindingsAsync();
    }
    public override async Task ContinueTurnAsync()
    {
        await base.ContinueTurnAsync();
        if (_model!.PlayerHand1!.IsEnabled == false && SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
        {
            _model.PlayerHand1.ReportCanExecuteChange();
        }
        //SingleInfo = PlayerList!.GetSelf();
        //WhoTurn = SingleInfo.Id;
        //this.ShowTurn();
        //if (SingleInfo.CardsPassed.Count == 0)
        //{
        //    await base.ContinueTurnAsync();
        //    return;
        //}
        //await SaveStateAsync();
        //if (BasicData!.MultiPlayer == false)
        //{
        //    throw new CustomBasicException("Rethink because computer would have pass cards");
        //}
        //Network!.IsEnabled = true;
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "takepointsaway":
                await GiveSelfMinusPointsAsync();
                break;
            case "givepointseverybodyelse":
                await GiveEverybodyElsePointsAsync();
                break;
            case "passcards":
                var thisList = await content.GetSavedIntegerListAsync();
                await CardsPassedAsync(thisList);
                break;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override async Task PopulateSaveRootAsync()
    {
        await base.PopulateSaveRootAsync();
        SaveRoot!.DummyList = _model!.Dummy1!.HandList.ToRegularDeckDict();
        SaveRoot.BlindList = _model.Blind1!.HandList.ToRegularDeckDict();
        if (SaveRoot.DummyList.Count != _model.Dummy1.HandList.Count)
        {
            throw new CustomBasicException("Dummy does not reconcile when populating the saveroot");
        }
        if (SaveRoot.BlindList.Count != 4)
        {
            throw new CustomBasicException("The blind must have 4 cards");
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
        SingleInfo = PlayerList.GetWhoPlayer();
        if (SaveRoot.GameStatus == EnumStatus.Passing)
        {
            if (SingleInfo.CardsPassed.Count == 3)
            {
                //this means done.
                await AfterCardsPassedAsync();
                return;
            }
        }
        this.ShowTurn();
        await SaveStateAsync();
        await ContinueTurnAsync();
    }
    //Task IFinishStart.FinishStartAsync()
    //{
    //    if (SaveRoot!.GameStatus == EnumStatus.Passing)
    //    {
    //        SingleInfo = PlayerList!.GetSelf();
    //        WhoTurn = SingleInfo.Id; //i think needs to be this way because its passing.
    //    }
    //    return Task.CompletedTask;
    //}
    private int WhoWonTrick(DeckRegularDict<HuseHeartsCardInformation> thisCol)
    {
        var leadCard = thisCol.First();
        var tempList = thisCol.ToRegularDeckDict();
        tempList.RemoveSpecificItem(leadCard);
        if (tempList.Any(x => x.Suit == leadCard.Suit && x.Value > leadCard.Value) == false)
        {
            return leadCard.Player;
        }
        return WhoTurn;
    }
    public override async Task EndTrickAsync()
    {
        var trickList = SaveRoot!.TrickList;
        if (trickList.Any(x => x.Suit == EnumSuitList.Hearts))
        {
            SaveRoot.TrickStatus = EnumTrickStatus.SuitBroken;
        }
        else if (SaveRoot.TrickStatus == EnumTrickStatus.FirstTrick)
        {
            SaveRoot.TrickStatus = EnumTrickStatus.NoSuit;
        }
        int wins = WhoWonTrick(trickList);
        HuseHeartsPlayerItem thisPlayer = PlayerList![wins];
        thisPlayer.TricksWon++;
        WhoTurn = wins;
        SingleInfo = PlayerList.GetWhoPlayer();
        int points = trickList.Sum(xx => xx.HeartPoints);
        if (SingleInfo.HadPoints == false)
        {
            SingleInfo.HadPoints = trickList.Any(Items => Items.ContainPoints == true);
        }
        if (points != 0)
        {
            SingleInfo.CurrentScore += points;
            if (SaveRoot.WhoWinsBlind == 0 && SingleInfo.HadPoints == true)
            {
                SaveRoot.WhoWinsBlind = wins;
                if (SingleInfo.PlayerCategory != EnumPlayerCategory.Self)
                {
                    _model!.Blind1!.Visible = false;
                }
                _model!.Blind1!.HandList.MakeAllObjectsKnown();
                _command.UpdateAll();
            }
        }
        await _aTrick!.AnimateWinAsync(wins);
        if (SingleInfo.MainHandList.Count == 0)
        {
            await EndRoundAsync();
            return; //most of the time its in rounds.
        }
        _model!.PlayerHand1!.EndTurn();
        SaveRoot.WhoLeadsTrick = WhoTurn;
        await StartNewTrickAsync();
    }
    private async Task StartNewTrickAsync()
    {
        _aTrick!.ClearBoard();
        _command!.ManuelFinish = true; //because it could be somebody else's turn.
        await StartNewTurnAsync(); //hopefully this simple.
    }
    Task IStartNewGame.ResetAsync()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.TotalScore = 0;
            thisPlayer.PreviousScore = 0;
        });
        SaveRoot!.RoundNumber = 0;
        return Task.CompletedTask;
    }
    //decided to not have separate for shooting moon or even passing cards for game class.
    public override async Task EndRoundAsync()
    {
        WhoTurn = SaveRoot!.WhoWinsBlind;
        if (SaveRoot.WhoWinsBlind == 0)
        {
            throw new CustomBasicException("Somebody has to win the blind eventually");
        }
        if (SaveRoot.BlindList.Count != 4)
        {
            throw new CustomBasicException("The blind has to have 4 cards at the end of the round");
        }
        if (PlayerList.Count != 2)
        {
            throw new CustomBasicException("Huse Hearts Is A 2 Player Game");
        }
        SingleInfo = PlayerList.GetWhoPlayer(); //this could be the fix for the problem.
        int points = SaveRoot.BlindList.Sum(xx => xx.HeartPoints);
        SingleInfo!.CurrentScore += points;
        var firstPlayer = PlayerList.First();
        var lastPlayer = PlayerList.Last();
        firstPlayer.PreviousScore = firstPlayer.CurrentScore;
        lastPlayer.PreviousScore = lastPlayer.CurrentScore;
        if (firstPlayer.PreviousScore + lastPlayer.PreviousScore != 26)
        {
            throw new CustomBasicException("The total points for the players has to be 26 points"); //was 16 but now 26 since the jack of diamonds is no longer -10 points.
        }
        int Shoots = PlayerList!.WhoShotMoon();
        if (Shoots == 0)
        {
            await FinishEndAsync();
            return;
        }
        WhoTurn = Shoots;
        SaveRoot.GameStatus = EnumStatus.ShootMoon;
        SingleInfo = PlayerList!.GetWhoPlayer();
        _model.Blind1.Visible = false; //has to be false.
        _toast.ShowInfoToast($"{SingleInfo.NickName}  has shot the moon.  The player needs to choose to either give 26 points to the other player or take 26 points off their own score");
        await StartNewTurnAsync();
    }
    private async Task FinishEndAsync()
    {
        SaveRoot!.GameStatus = EnumStatus.EndRound;
        PlayerList!.ForEach(thisPlayer => thisPlayer.TotalScore += thisPlayer.CurrentScore);
        if (CanEndGame() == true)
        {
            await ShowWinAsync();
            return;
        }
        await this.RoundOverNextAsync();
    }
    private void GetWinningPlayer()
    {
        SingleInfo = PlayerList.OrderBy(items => items.TotalScore).Take(1).Single();
    }
    private bool CanEndGame()
    {
        if (SaveRoot!.RoundNumber >= 12)
        {
            GetWinningPlayer();
            return true;
        }
        if (PlayerList.Any(items => items.TotalScore >= 100))
        {
            GetWinningPlayer();
            return true;
        }
        return false;
    }
    internal async Task GiveSelfMinusPointsAsync()
    {
        SingleInfo!.CurrentScore -= 52;
        SingleInfo.PreviousScore = SingleInfo.CurrentScore;
        _command.UpdateAll();
        if (SingleInfo.PlayerCategory == EnumPlayerCategory.OtherHuman)
        {
            await _message.ShowMessageAsync($"{SingleInfo.NickName} chose to give him/herself -26 points");
        }
        await FinishEndAsync();
    }
    internal async Task GiveEverybodyElsePointsAsync()
    {
        int newPlayer;
        if (WhoTurn == 1)
        {
            newPlayer = 2;
        }
        else
        {
            newPlayer = 1;
        }
        var thisPlayer = PlayerList![newPlayer];
        SingleInfo!.CurrentScore -= 26;
        SingleInfo.PreviousScore = SingleInfo.CurrentScore;
        thisPlayer.CurrentScore += 26;
        thisPlayer.PreviousScore = thisPlayer.CurrentScore;
        _command.UpdateAll();
        if (SingleInfo.PlayerCategory == EnumPlayerCategory.OtherHuman)
        {
            await _message.ShowMessageAsync($"{SingleInfo.NickName} chose to give the other player 26 points");
        }
        await FinishEndAsync();
    }
    private void TransferToPassed(BasicList<int> thisList)
    {
        SingleInfo!.CardsPassed = thisList;
        thisList.ForEach(index => SingleInfo.MainHandList.RemoveObjectByDeck(index));
    }
    private async Task ComputerPassCardsAsync()
    {
        if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Computer)
        {
            throw new CustomBasicException("Must be computer player when the computer is passing cards");
        }
        var thisList = SingleInfo.MainHandList.GetRandomList(false, 3);
        await CardsPassedAsync(thisList.Select(items => items.Deck).ToBasicList());
    }
    public async Task CardsPassedAsync(BasicList<int> thisList)
    {
        if (thisList.Count != 3)
        {
            throw new CustomBasicException("Must pass 3 cards");
        }
        SingleInfo = PlayerList!.GetWhoPlayer(); //has to be whoever turn it is.
        TransferToPassed(thisList);
        await EndTurnAsync();
    }
    private async Task AfterCardsPassedAsync()
    {
        if (PlayerList.Any(xx => xx.CardsPassed.Count != 3))
        {
            throw new CustomBasicException("All players must have passed 3 cards");
        }
        var firstPlayer = PlayerList.First();
        var secondPlayer = PlayerList.Last();
        HuseHeartsCardInformation card;
        DeckRegularDict<HuseHeartsCardInformation> firstList = [];
        DeckRegularDict<HuseHeartsCardInformation> secondList = [];
        firstPlayer.CardsPassed.ForEach(deck =>
        {
            card = _gameContainer.DeckList!.GetSpecificItem(deck);
            card.IsSelected = false;
            card.Drew = true;
            secondList.Add(card);
        });
        secondPlayer.MainHandList.AddRange(secondList);
        secondPlayer.CardsPassed.ForEach(deck =>
        {
            card = _gameContainer.DeckList!.GetSpecificItem(deck);
            card.IsSelected = false;
            card.Drew = true;
            firstList.Add(card);
        });
        firstPlayer.MainHandList.AddRange(firstList);
        SortCards();
        if (firstPlayer.MainHandList.Count != 16 || secondPlayer.MainHandList.Count != 16)
        {
            throw new CustomBasicException("All players must have 16 cards in hand after passing");
        }
        firstPlayer.CardsPassed.Clear();
        secondPlayer.CardsPassed.Clear();
        SaveRoot!.WhoLeadsTrick = WhoLeadsFirstTrick();
        SaveRoot.GameStatus = EnumStatus.Normal;
        _model.TrickArea1!.ClearBoard();
        WhoTurn = SaveRoot.WhoLeadsTrick;
        SingleInfo = PlayerList!.GetWhoPlayer();
        await StartNewTurnAsync();
    }
    private int WhoLeadsFirstTrick()
    {
        var tempList = PlayerList!.CardsFromAllPlayers<HuseHeartsCardInformation, HuseHeartsPlayerItem>();
        int tempDeck;
        if (tempList.Any(Items => Items.Suit == EnumSuitList.Clubs))
        {
            tempDeck = tempList.Where(xx => xx.Suit == EnumSuitList.Clubs).OrderBy(xx => xx.Value).First().Deck;
        }
        else
        {
            tempDeck = tempList.Where(xx => xx.Suit == EnumSuitList.Diamonds).OrderBy(xx => xx.Value).First().Deck;
        }
        return PlayerList!.WhoHasCardFromDeck<HuseHeartsCardInformation, HuseHeartsPlayerItem>(tempDeck);
    }
}