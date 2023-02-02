namespace PickelCardGame.Core.Logic;
[SingletonGame]
public class PickelCardGameMainGameClass
    : TrickGameClass<EnumSuitList, PickelCardGameCardInformation, PickelCardGamePlayerItem, PickelCardGameSaveInfo>
        , IMiscDataNM, IStartNewGame, ISerializable
{
    private readonly PickelCardGameVMData _model;
    private readonly CommandContainer _command;
    private readonly IAdvancedTrickProcesses _aTrick;
    private readonly IBidProcesses _processes;
    public PickelCardGameMainGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        PickelCardGameVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<PickelCardGameCardInformation> cardInfo,
        CommandContainer command,
        PickelCardGameGameContainer gameContainer,
        ITrickData trickData,
        ITrickPlay trickPlay,
        IAdvancedTrickProcesses aTrick,
        IBidProcesses processes,
        ISystemError error,
        IToast toast
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, trickData, trickPlay, error, toast)
    {
        _model = currentMod;
        _command = command;
        gameContainer.StartNewTrickAsync = StartNewTrickAsync;
        _aTrick = aTrick;
        _processes = processes;
    }
    public override bool CanMakeMainOptionsVisibleAtBeginning => CanEnableTrickAreas;
    public override bool CanEnableTrickAreas => SaveRoot!.GameStatus != EnumStatusList.Bidding;
    public override async Task FinishGetSavedAsync()
    {
        LoadControls();
        LoadVM();
        if (SaveRoot!.GameStatus == EnumStatusList.Bidding)
        {
            await _processes!.PopulateBidsAsync();
        }
        await base.FinishGetSavedAsync();
        _aTrick!.LoadGame();
    }
    protected override async Task ComputerTurnAsync()
    {
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelaySeconds(.25);
        }
        if (SaveRoot!.GameStatus == EnumStatusList.Bidding)
        {
            int bids = ComputerAI.HowManyToBid(_model, _processes);
            if (bids == -1)
            {
                await _processes.PassBidAsync();
                return;
            }
            if (bids == 0)
            {
                throw new CustomBasicException("The computer cannot choose 0 bids.");
            }
            var suit = ComputerAI.SuitToCall(_model);
            _processes!.SelectBidAndSuit(bids, suit);
            await _processes.ProcessBidAsync();
            return;
        }
        int deck = ComputerAI.CardToPlay(this);
        await PlayCardAsync(deck);
    }
    private void LoadControls()
    {
        if (IsLoaded == true)
        {
            return;
        }
        IsLoaded = true;
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        LoadControls();
        LoadVM();
        SaveRoot!.GameStatus = EnumStatusList.Bidding;
        SaveRoot.CardList.Clear();
        SaveRoot.WonSoFar = 0;
        if (PlayerList.Count == 2)
        {
            SaveRoot.HighestBid = 3;
        }
        else if (PlayerList.Count == 3)
        {
            SaveRoot.HighestBid = 2;
        }
        else
        {
            throw new CustomBasicException("Only 2 or 3 players are supported");
        }
        PlayerList!.ForEach(player =>
        {
            player.SuitDesired = EnumSuitList.None;
            player.BidAmount = 0;
            player.TricksWon = 0;
        });
        SaveRoot.TrumpSuit = EnumSuitList.None;
        return base.StartSetUpAsync(isBeginning);
    }
    protected override async Task LastPartOfSetUpBeforeBindingsAsync()
    {
        await _processes.PopulateBidsAsync();
        await base.LastPartOfSetUpBeforeBindingsAsync();
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "pass":
                await _processes.PassBidAsync();
                return;
            case "bid":
                SendBid thisBid = await js1.DeserializeObjectAsync<SendBid>(content);
                _processes!.SelectBidAndSuit(thisBid.Bid, thisBid.Suit);
                await _processes.ProcessBidAsync();
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
    public override bool IsValidMove(int deck)
    {
        if (SaveRoot!.TrickList.Count == 0)
        {
            return true;
        }
        var OriginalCard = SaveRoot.TrickList.First();
        if (OriginalCard.CardType == EnumRegularCardTypeList.Joker)
        {
            return true;
        }
        return base.IsValidMove(deck);
    }
    private int WhoWonTrick(DeckRegularDict<PickelCardGameCardInformation> thisCol)
    {
        var firstItem = thisCol.Where(x => x.Suit == SaveRoot!.TrumpSuit).OrderByDescending(xx => xx.Value).FirstOrDefault();
        if (firstItem != null)
        {
            return firstItem.Player;
        }
        DeckRegularDict<PickelCardGameCardInformation> tempCol;
        int x;
        PickelCardGameCardInformation thisCard;
        if (thisCol.Any(x => x.CardType == EnumRegularCardTypeList.Joker))
        {
            EnumRegularCardValueList Nums = EnumRegularCardValueList.None;
            int firstPlayer;
            firstItem = thisCol.Where(x => x.CardType != EnumRegularCardTypeList.Joker).OrderByDescending(xx => xx.Value).FirstOrDefault();
            if (firstItem != null)
            {
                Nums = firstItem.Value;
                firstPlayer = firstItem.Player;
                if (Nums <= EnumRegularCardValueList.Nine)
                {
                    tempCol = thisCol.Where(x => x.CardType == EnumRegularCardTypeList.Joker).ToRegularDeckDict();
                    if (tempCol.Count == 1)
                    {
                        return tempCol.Single().Player;
                    }
                }
            }
            for (x = thisCol.Count; x >= 1; x += -1)
            {
                thisCard = thisCol[x - 1];
                if (thisCard.CardType == EnumRegularCardTypeList.Joker)
                {
                    return thisCard.Player;
                }
            }
            tempCol = thisCol.Where(x => x.CardType != EnumRegularCardTypeList.Joker && x.Value == Nums).ToRegularDeckDict();
            if (tempCol.Count == 1)
            {
                return tempCol.Single().Player;
            }
            for (x = thisCol.Count; x >= 1; x += -1)
            {
                thisCard = thisCol[x - 1];
                if (thisCard.Value == Nums)
                {
                    return thisCard.Player;
                }
            }
            throw new CustomBasicException($"Did not find anyone with the highest card of {Nums}.  Find out what happened");
        }
        EnumSuitList leadSuit = thisCol.First().Suit;
        return thisCol.Where(x => x.Suit == leadSuit).OrderByDescending(xx => xx.Value).First().Player;
    }
    public override async Task EndTrickAsync()
    {
        var trickList = SaveRoot!.TrickList;
        int wins = WhoWonTrick(trickList);
        PickelCardGamePlayerItem thisPlayer = PlayerList![wins];
        thisPlayer.TricksWon++;
        await _aTrick!.AnimateWinAsync(wins);
        trickList.ForEach(thisCard =>
        {
            PickelCardGameCardInformation newCard = new();
            newCard.Populate(thisCard.Deck);
            newCard.Player = wins;
            SaveRoot.CardList.Add(newCard);
        });
        if (SaveRoot.CardList.Count > 54)
        {
            throw new CustomBasicException("The cardlist can't have more than 54 cards");
        }
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
            thisPlayer.CurrentScore = 0;
            thisPlayer.TotalScore = 0;
        });
        return Task.CompletedTask;
    }
    private bool CanEndGame()
    {
        if (PlayerList.Any(items => items.TotalScore >= 300) == false)
        {
            return false;
        }
        SingleInfo = PlayerList.OrderByDescending(items => items.TotalScore).First();
        return true;
    }
    private void FirstScoring()
    {
        var thisPlayer = PlayerList![SaveRoot!.WonSoFar];
        int tricksReceived = thisPlayer.TricksWon;
        int amountBidded = thisPlayer.BidAmount;
        if (amountBidded == 0)
        {
            throw new CustomBasicException("The amount bidded cannot be 0");
        }
        int totalPoints;
        if (tricksReceived >= amountBidded)
        {
            int diffs = tricksReceived - amountBidded;
            int bonus;
            if (amountBidded == 8)
            {
                bonus = 10;
            }
            else if (amountBidded == 9)
            {
                bonus = 30;
            }
            else if (amountBidded == 10)
            {
                bonus = 50;
            }
            else
            {
                bonus = 0;
            }
            int extras = 0;
            if (diffs > 0)
            {
                extras = diffs * 2;
            }
            int normalPoints = amountBidded * 5;
            totalPoints = normalPoints + bonus + extras;
            thisPlayer.CurrentScore = totalPoints;
            thisPlayer.TotalScore += totalPoints;
            return;
        }
        PlayerList.ForEach(otherPlayer =>
        {
            if (otherPlayer.Equals(thisPlayer) == false)
            {
                totalPoints = otherPlayer.TricksWon * 5;
                otherPlayer.CurrentScore = totalPoints;
                otherPlayer.TotalScore += totalPoints;
            }
        });
    }
    private int PointsForPlayer(int player)
    {
        return SaveRoot!.CardList.Where(items => items.Player == player).Sum(Items => Items.Points);
    }
    private void SecondScoring()
    {
        CardScoring();
        int points;
        PlayerList!.ForEach(thisPlayer =>
        {
            points = PointsForPlayer(thisPlayer.Id);
            thisPlayer.CurrentScore = points;
            thisPlayer.TotalScore += points;
        });
    }
    private void CardScoring()
    {
        SaveRoot!.CardList.ForEach(items => items.Points = 0);
        PickelCardGameCardInformation tempCard = SaveRoot.CardList.Where(items => items.Suit == SaveRoot.TrumpSuit).OrderBy(Items => Items.Value).FirstOrDefault()!;
        int decks = 0;
        if (tempCard != null)
        {
            decks = tempCard.Deck;
            tempCard.Points = 15;
        }
        SaveRoot.CardList.ForEach(thisCard =>
        {
            if (thisCard.CardType == EnumRegularCardTypeList.Joker)
            {
                thisCard.Points = 15;
            }
            else if (thisCard.Deck == decks)
            {
                if (thisCard.Points != 15)
                {
                    throw new CustomBasicException("Did not update the points.  Find out what happened");
                }
            }
            else if (thisCard.Value == EnumRegularCardValueList.Five)
            {
                thisCard.Points = 5;
            }
            else if (thisCard.Value == EnumRegularCardValueList.Ten)
            {
                thisCard.Points = 10;
            }
            else if (thisCard.Value == EnumRegularCardValueList.HighAce)
            {
                thisCard.Points = 4;
            }
            else if (thisCard.Value == EnumRegularCardValueList.Jack)
            {
                thisCard.Points = 1;
            }
            else if (thisCard.Value == EnumRegularCardValueList.Queen)
            {
                thisCard.Points = 2;
            }
            else if (thisCard.Value == EnumRegularCardValueList.King)
            {
                thisCard.Points = 3;
            }
            else
            {
                thisCard.Points = 0;
            }
        });
    }
    public override async Task EndRoundAsync()
    {
        PlayerList!.ForEach(thisPlayer => thisPlayer.CurrentScore = 0);
        FirstScoring();
        if (CanEndGame() == true)
        {
            await ShowWinAsync();
            return;
        }
        SecondScoring();
        if (CanEndGame() == true)
        {
            await ShowWinAsync();
            return;
        }
        await this.RoundOverNextAsync();
    }
}