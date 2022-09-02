namespace RageCardGame.Core.Logic;
[SingletonGame]
public class RageCardGameMainGameClass
    : TrickGameClass<EnumColor, RageCardGameCardInformation, RageCardGamePlayerItem, RageCardGameSaveInfo>
        , IMiscDataNM, IStartNewGame, ISerializable
{
    private readonly RageCardGameVMData _model;
    private readonly CommandContainer _command;
    private readonly RageCardGameGameContainer _gameContainer;
    private readonly IAdvancedTrickProcesses _aTrick;
    private readonly IColorProcesses _colorProcesses;
    private readonly IBidProcesses _bidProcesses;
    private readonly RageDelgates _delgates;
    public RageCardGameMainGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        RageCardGameVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<RageCardGameCardInformation> cardInfo,
        CommandContainer command,
        RageCardGameGameContainer gameContainer,
        ITrickData trickData,
        ITrickPlay trickPlay,
        IAdvancedTrickProcesses aTrick,
        IColorProcesses colorProcesses,
        IBidProcesses bidProcesses,
        RageDelgates delgates,
        ISystemError error,
        IToast toast
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, trickData, trickPlay, error, toast)
    {
        _model = currentMod;
        _command = command;
        _gameContainer = gameContainer;
        _aTrick = aTrick;
        _colorProcesses = colorProcesses;
        _bidProcesses = bidProcesses;
        _delgates = delgates;
        delgates.CardsToPassOut = () => SaveRoot.CardsToPassOut;
    }
    public override async Task FinishGetSavedAsync()
    {
        LoadControls();
        LoadVM();
        await base.FinishGetSavedAsync();
        _aTrick!.LoadGame();
        RevealBids();
    }
    public override bool CanEnableTrickAreas => SaveRoot!.Status == EnumStatus.Regular;
    private void RevealBids()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.RevealBid = thisPlayer.PlayerCategory == EnumPlayerCategory.Self || thisPlayer.MainHandList.Count == 1;
        });
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
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelaySeconds(.75);
        }
        if (SaveRoot!.Status == EnumStatus.Bidding)
        {
            _model!.BidAmount = _model.Bid1!.NumberToChoose();
            await _bidProcesses.ProcessBidAsync();
            return;
        }
        if (SaveRoot.Status == EnumStatus.ChooseColor)
        {
            _model!.ColorChosen = _model.Color1!.ItemToChoose();
            await _colorProcesses.ColorChosenAsync();
            return;
        }
        var thisList = SingleInfo!.MainHandList.Where(items => IsValidMove(items.Deck)).Select(Items => Items.Deck).ToBasicList();
        await PlayCardAsync(thisList.GetRandomItem());
    }
    public override async Task ContinueTurnAsync()
    {
        if (SaveRoot!.Status == EnumStatus.ChooseColor)
        {
            await _colorProcesses.LoadColorListsAsync();
        }
        else if (SaveRoot.Status == EnumStatus.Bidding)
        {
            await _bidProcesses.LoadBiddingScreenAsync();
        }
        await base.ContinueTurnAsync();
    }
    protected override Task LastPartOfSetUpBeforeBindingsAsync()
    {
        int ask1 = _gameContainer.Random.GetRandomNumber(6);
        SaveRoot!.TrumpSuit = EnumColor.FromValue(ask1);
        return base.LastPartOfSetUpBeforeBindingsAsync();
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        if (isBeginning == true)
        {
            SaveRoot!.CardsToPassOut = 11;
        }
        else
        {
            SaveRoot!.CardsToPassOut--;
        }
        LoadControls();
        LoadVM();
        SaveRoot.CardList.Clear();
        SaveRoot.Status = EnumStatus.Bidding;
        RevealBids();
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.BidAmount = -1;
            thisPlayer.TricksWon = 0;
        });
        return base.StartSetUpAsync(isBeginning);
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "color":
                _model!.ColorChosen = await js.DeserializeObjectAsync<EnumColor>(content);
                await _colorProcesses.ColorChosenAsync();
                break;
            case "bid":
                _model!.BidAmount = int.Parse(content);
                await _bidProcesses.ProcessBidAsync();
                break;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override async Task StartNewTurnAsync()
    {
        await base.StartNewTurnAsync();
        await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
    }
    public override async Task EndTurnAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        SingleInfo.MainHandList.UnhighlightObjects();
        _command.ManuelFinish = true; //because it could be somebody else's turn.
        WhoTurn = await PlayerList.CalculateWhoTurnAsync();
        if (SaveRoot!.Status == EnumStatus.Bidding && WhoTurn == WhoStarts)
        {
            SaveRoot.Status = EnumStatus.Regular;
            if (_delgates.CloseBidScreenAsync == null)
            {
                throw new CustomBasicException("Nobody is closing the bidding screen.  Rethink");
            }
            await _delgates.CloseBidScreenAsync.Invoke();
            await StartNewTrickAsync();
            return;
        }
        await StartNewTurnAsync();
    }
    public override bool IsValidMove(int deck)
    {
        if (deck == 0)
        {
            throw new CustomBasicException("Deck cannot be 0 for IsValidMove");
        }
        var thisList = SaveRoot!.TrickList;
        if (thisList.Count == 0)
        {
            return true;
        }
        var LeadCard = thisList.FirstOrDefault(Items => Items.Color != EnumColor.None);
        if (LeadCard == null)
        {
            return true;
        }
        var cardPlayed = _gameContainer.DeckList!.GetSpecificItem(deck);
        if (cardPlayed.Color == LeadCard.Color)
        {
            return true;
        }
        return base.IsValidMove(deck);
    }
    private int WhoWonTrick(DeckRegularDict<RageCardGameCardInformation> thisCol)
    {
        if (thisCol.Any(Items => Items.SpecialType == EnumSpecialType.Wild && Items.Color == EnumColor.None))
        {
            throw new CustomBasicException("Must have chosen a color for the wild");
        }
        var tempList = thisCol.Where(items => items.Color != EnumColor.None).ToRegularDeckDict();
        if (tempList.Count == 0)
        {
            return thisCol.First().Player;
        }
        var leadColor = tempList.First().Color;
        var trumpList = tempList.Where(items => items.Color == SaveRoot!.TrumpSuit).ToRegularDeckDict();
        if (trumpList.Count == 0)
        {
            return tempList.Where(items => items.Color == leadColor).OrderByDescending(Items => Items.Value).Take(1).Single().Player;
        }
        return trumpList.OrderByDescending(Items => Items.Value).Take(1).Single().Player;
    }
    public override bool CanMakeMainOptionsVisibleAtBeginning => SaveRoot.Status == EnumStatus.Regular;
    public override async Task EndTrickAsync()
    {
        var trickList = SaveRoot!.TrickList;
        int wins = WhoWonTrick(trickList);
        RageCardGamePlayerItem thisPlayer = PlayerList![wins];
        thisPlayer.TricksWon++;
        await _aTrick!.AnimateWinAsync(wins);
        trickList.ForEach(thisCard =>
        {
            thisCard.Player = wins;
            SaveRoot.CardList.Add(thisCard);
        });
        if (SingleInfo!.MainHandList.Count == 0)
        {
            await EndRoundAsync();
            return;
        }
        _model!.PlayerHand1!.EndTurn();
        RevealBids();
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
        SaveRoot!.CardsToPassOut = 12; //try 12 since it will reduce by one.
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.ScoreGame = 0;
            thisPlayer.ScoreRound = 0;
            thisPlayer.CorrectlyBidded = 0;
        });
        return Task.CompletedTask;
    }
    private void CalculateScore()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.ScoreRound = 0;
            int extras;
            if (thisPlayer.TricksWon == thisPlayer.BidAmount)
            {
                thisPlayer.CorrectlyBidded++;
                if (thisPlayer.BidAmount == 0)
                {
                    extras = 5;
                }
                else
                {
                    extras = 10;
                }
            }
            else
            {
                extras = 0;
            }
            thisPlayer.ScoreRound += extras;
            int score = SaveRoot!.CardList.Where(items => items.Player == thisPlayer.Id).Sum(Items => Items.GetPoints);
            thisPlayer.ScoreRound += score;
            if (thisPlayer.BidAmount > 0)
            {
                int wons;
                if (thisPlayer.TricksWon == SaveRoot.CardsToPassOut)
                {
                    wons = SaveRoot.CardsToPassOut * 2;
                }
                else
                {
                    wons = thisPlayer.TricksWon;
                }
                thisPlayer.ScoreRound += wons;
            }
            thisPlayer.ScoreGame += thisPlayer.ScoreRound;
        });
    }
    public override async Task EndRoundAsync()
    {
        CalculateScore();
        if (SaveRoot!.CardsToPassOut == 1)
        {
            await GameOverAsync();
            return;
        }
        await this.RoundOverNextAsync();
    }
    private async Task GameOverAsync()
    {
        SetWinPlayer();
        await ShowWinAsync();
    }
    private void SetWinPlayer()
    {
        var possibleItem = PlayerList.OrderByDescending(xx => xx.ScoreGame).Take(1).Single();
        if (PlayerList.Count(xx => xx.ScoreGame == possibleItem.ScoreGame) == 1)
        {
            SingleInfo = possibleItem;
            return;
        }
        var tieList = PlayerList.Where(xx => xx.ScoreGame == possibleItem.ScoreGame).ToBasicList();
        if (tieList.Count == 0)
        {
            throw new CustomBasicException("No Tie");
        }
        SingleInfo = tieList.OrderByDescending(xx => xx.CorrectlyBidded).First();
    }
}