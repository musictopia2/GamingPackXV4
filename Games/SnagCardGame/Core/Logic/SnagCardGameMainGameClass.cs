namespace SnagCardGame.Core.Logic;
[SingletonGame]
public class SnagCardGameMainGameClass
    : TrickGameClass<EnumSuitList, SnagCardGameCardInformation, SnagCardGamePlayerItem, SnagCardGameSaveInfo>
        , IMiscDataNM, IStartNewGame, ISerializable
{
    private readonly SnagCardGameVMData _model;
    private readonly CommandContainer _command;
    private readonly SnagCardGameGameContainer _gameContainer;
    private readonly IAdvancedTrickProcesses _aTrick;
    public SnagCardGameMainGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        SnagCardGameVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<SnagCardGameCardInformation> cardInfo,
        CommandContainer command,
        SnagCardGameGameContainer gameContainer,
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
        _gameContainer.TakeCardAsync = TakeCardAsync;
        _aTrick = aTrick;
    }
    public override async Task FinishGetSavedAsync()
    {
        LoadControls();
        LoadVM();
        ShowBasicControls();
        _model.Bar1.LoadSavedGame();
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
    internal async Task TakeCardAsync(int Deck)
    {
        if (Deck == 0)
        {
            throw new CustomBasicException("The deck cannot be 0 when taking card");
        }
        if (WhoTurn == 0)
        {
            throw new CustomBasicException("The turn cannot be 0 when taking card");
        }
        if (SingleInfo!.CanSendMessage(BasicData!) == true)
        {
            await Network!.SendAllAsync("cardchosen", Deck);
        }
        _model.TrickArea1!.SelectCard(Deck);
        _command.UpdateAll();
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelaySeconds(.5);
        }
        var thisCard = _gameContainer.DeckList!.GetSpecificItem(Deck);
        thisCard.Player = WhoTurn;
        SaveRoot!.CardList.Add(thisCard);
        _model.TrickArea1.RemoveCard(Deck);
        SingleInfo!.CardsWon++;
        int lefts = _model.TrickArea1.HowManyCardsLeft;
        if (lefts == 1)
        {
            int final = _model.TrickArea1.GetLastCard;
            thisCard = _gameContainer.DeckList.GetSpecificItem(final);
            if (WhoTurn == 1)
            {
                thisCard.Player = 2;
            }
            else
            {
                thisCard.Player = 1;
            }
            var thisPlayer = PlayerList![thisCard.Player];
            thisPlayer.CardsWon++;
            SaveRoot.CardList.Add(thisCard);
            if (_model!.Bar1!.HandList.Count > 0)
            {
                WhoTurn = thisCard.Player;
                SingleInfo = PlayerList.GetWhoPlayer();
                await StartNewTrickAsync();
                return;
            }
            await EndRoundAsync();
            return;
        }
        await EndTurnAsync();
    }
    public override Task PopulateSaveRootAsync()
    {
        SaveRoot!.BarList = _model.Bar1.HandList.ToRegularDeckDict();
        return base.PopulateSaveRootAsync();
    }
    protected override async Task ComputerTurnAsync()
    {
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelaySeconds(.25);
        }
        if (SaveRoot!.GameStatus == EnumStatusList.Normal)
        {
            await PlayCardAsync(ComputerAI.CardToPlay(this, _model));
            return;
        }
        await TakeCardAsync(ComputerAI.CardToTake(_model));
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        LoadControls();
        LoadVM();
        if (isBeginning == false)
        {
            ShowBasicControls();
        }
        SaveRoot!.CardList.Clear();
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.CardsWon = 0;
        });
        ResetCards();
        return base.StartSetUpAsync(isBeginning);
    }
    private void ResetCards()
    {
        SaveRoot!.GameStatus = EnumStatusList.Normal;
        SaveRoot.FirstCardPlayed = false;
    }
    protected override Task LastPartOfSetUpBeforeBindingsAsync()
    {
        _model.TrickArea1!.ClearBoard();
        _model.Bar1.LoadBarCards(CardInfo!.DummyHand);
        return base.LastPartOfSetUpBeforeBindingsAsync();
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "cardchosen":
                await TakeCardAsync(int.Parse(content));
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
        if (SaveRoot!.FirstCardPlayed == false)
        {
            SaveRoot.FirstCardPlayed = true;
        }
        _command.ManuelFinish = true; //because it could be somebody else's turn.
        WhoTurn = await PlayerList.CalculateWhoTurnAsync();
        this.ShowTurn();
        await SaveStateAsync();
        await ContinueTurnAsync();
    }
    public override Task ContinueTurnAsync()
    {
        if (SaveRoot!.FirstCardPlayed == false)
        {
            _model!.Instructions = "Choose a card from the bar to start the trick";
        }
        else if (SaveRoot.GameStatus == EnumStatusList.Normal)
        {
            _model!.Instructions = "Choose a card from the hand";
        }
        else
        {
            _model!.Instructions = "Choose a card to take";
        }
        return base.ContinueTurnAsync();
    }
    private static int WhoWonTrick(DeckRegularDict<SnagCardGameCardInformation> thisCol)
    {
        int begins = thisCol.First().Player;
        EnumRegularCardValueList highestNumber = thisCol.OrderByDescending(items => items.Value).First().Value;
        SnagCardGameCardInformation thisCard;
        int x;
        if (thisCol.Count(items => items.Value == highestNumber) == 1)
        {
            for (x = 1; x <= 3; x++) //no ties
            {
                thisCard = thisCol[x - 1];
                if (thisCard.Value == highestNumber && (x == 1 || x == 3))
                {
                    return begins;
                }
                else if (thisCard.Value == highestNumber)
                {
                    if (begins == 1)
                    {
                        return 2;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
        }
        for (x = 3; x >= 1; x += -1) //ties
        {
            thisCard = thisCol[x - 1];
            if ((thisCard.Value == highestNumber) & ((x == 1) | (x == 3)))
            {
                return begins;
            }
            else if (thisCard.Value == highestNumber)
            {
                if (begins == 1)
                {
                    return 2;
                }
                else
                {
                    return 1;
                }
            }
        }
        throw new CustomBasicException("Could not find who won");
    }
    public override async Task EndTrickAsync()
    {
        var trickList = SaveRoot!.TrickList;
        int wins = WhoWonTrick(trickList);
        WhoTurn = wins;
        SaveRoot.GameStatus = EnumStatusList.ChooseCards;
        await StartNewTurnAsync();
    }
    private async Task StartNewTrickAsync()
    {
        _aTrick!.ClearBoard();
        ResetCards();
        _command!.ManuelFinish = true; //because it could be somebody else's turn.
        await StartNewTurnAsync();
    }
    private void ShowBasicControls()
    {
        _model!.PlayerHand1!.Visible = true;
        _model.Opponent1!.Visible = false;
        _model.Human1!.Visible = false;
    }
    Task IStartNewGame.ResetAsync()
    {
        ShowBasicControls();
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.CurrentPoints = 0;
            thisPlayer.TotalPoints = 0;
        });
        return Task.CompletedTask;
    }
    private void Scoring()
    {
        DeckRegularDict<SnagCardGameCardInformation> firstCol = SaveRoot!.CardList.Where(items => items.Player == 1).ToRegularDeckDict();
        DeckRegularDict<SnagCardGameCardInformation> secondCol = SaveRoot.CardList.Where(items => items.Player == 2).ToRegularDeckDict();
        int wins = WhoWonRound(firstCol, secondCol, out int suits);
        int points = HowManyPoints(suits);
        var firstPlayer = PlayerList.First();
        var secondPlayer = PlayerList.Last();
        if (wins == 1)
        {
            firstPlayer.CurrentPoints = points;
            secondPlayer.CurrentPoints = 0;
            firstPlayer.TotalPoints += points;
        }
        else
        {
            secondPlayer.CurrentPoints = points;
            secondPlayer.TotalPoints += points;
            firstPlayer.CurrentPoints = 0;
        }
        DeckRegularDict<SnagCardGameCardInformation> humanList = new();
        DeckRegularDict<SnagCardGameCardInformation> opponentList = new();
        firstCol.ForEach(thisCard =>
        {
            thisCard.IsSelected = false;
            if (_gameContainer.SelfPlayer == 1)
            {
                humanList.Add(thisCard);
            }
            else
            {
                opponentList.Add(thisCard);
            }
        });
        secondCol.ForEach(thisCard =>
        {
            thisCard.IsSelected = false;
            if (_gameContainer.SelfPlayer == 1)
            {
                opponentList.Add(thisCard);
            }
            else
            {
                humanList.Add(thisCard);
            }
        });
        _model!.Human1!.HandList.ReplaceRange(humanList);
        _model.Opponent1!.HandList.ReplaceRange(opponentList);
        _model.PlayerHand1!.Visible = false;
        _model.Human1.Visible = true;
        _model.Opponent1.Visible = true;
        _model.TrickArea1!.Visible = false;
    }
    private static int HowManyPoints(int numberSuits)
    {
        int output = 0;
        numberSuits.Times(x =>
        {
            output += x;
        });
        return output;
    }
    private static void SortCards(DeckRegularDict<SnagCardGameCardInformation> firstCol, DeckRegularDict<SnagCardGameCardInformation> secondCol)
    {
        var temps = firstCol.GroupOrderDescending(items => items.Suit).ToBasicList();
        EnumSuitList newSuit;
        DeckRegularDict<SnagCardGameCardInformation> newCol = new();
        temps.ForEach(thisItem =>
        {
            newSuit = thisItem.Key;
            newCol.AddRange(firstCol.Where(items => items.Suit == newSuit));
        });
        firstCol.ReplaceRange(newCol);
        newCol.Clear();
        temps = secondCol.GroupOrderDescending(items => items.Suit).ToBasicList();
        temps.ForEach(ThisItem =>
        {
            newSuit = ThisItem.Key;
            newCol.AddRange(secondCol.Where(items => items.Suit == newSuit));
        });
        secondCol.ReplaceRange(newCol);
    }
    private static int WhoWonRound(DeckRegularDict<SnagCardGameCardInformation> firstCol, DeckRegularDict<SnagCardGameCardInformation> secondCol, out int numberOfSuits)
    {
        var firstTemp = firstCol.GroupOrderDescending(items => items.Suit).ToBasicList();
        var secondTemp = secondCol.GroupOrderDescending(items => items.Suit).ToBasicList();
        int x = 0;
        do
        {
            if (firstTemp.Count > secondTemp.Count && x == secondTemp.Count)
            {
                numberOfSuits = firstTemp[x].Count();
                SortCards(firstCol, secondCol);
                return 1;
            }
            if (secondTemp.Count > firstTemp.Count && x == firstTemp.Count)
            {
                numberOfSuits = secondTemp[x].Count();
                SortCards(firstCol, secondCol);
                return 2;
            }
            if (firstTemp[x].Count() > secondTemp[x].Count())
            {
                numberOfSuits = firstTemp[x].Count();
                SortCards(firstCol, secondCol);
                return 1;
            }
            if (secondTemp[x].Count() > firstTemp[x].Count())
            {
                numberOfSuits = secondTemp[x].Count();
                SortCards(firstCol, secondCol);
                return 2;
            }
            x++;
        } while (true);
    }
    public override async Task EndRoundAsync()
    {
        _model!.Instructions = "None";
        Scoring();
        if (CanEndGame() == true)
        {
            await ShowWinAsync();
            return;
        }
        await this.RoundOverNextAsync();
    }
    private bool CanEndGame()
    {
        SnagCardGamePlayerItem tempPlayer = PlayerList.Where(items => items.TotalPoints >= 50).OrderByDescending(xx => xx.TotalPoints).FirstOrDefault()!;
        if (tempPlayer == null)
        {
            return false;
        }
        SingleInfo = tempPlayer;
        return true;
    }
}