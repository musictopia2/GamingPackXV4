namespace FourSuitRummy.Core.Logic;
[SingletonGame]
public class FourSuitRummyMainGameClass
    : CardGameClass<RegularRummyCard, FourSuitRummyPlayerItem, FourSuitRummySaveInfo>
    , IMiscDataNM, IStartNewGame, ISerializable
{
    private readonly FourSuitRummyVMData _model;
    private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
    private readonly FourSuitRummyGameContainer _gameContainer; //if we don't need it, take it out.
    public FourSuitRummyMainGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        FourSuitRummyVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<RegularRummyCard> cardInfo,
        CommandContainer command,
        FourSuitRummyGameContainer gameContainer,
        ISystemError error,
        IToast toast
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, error, toast)
    {
        _model = currentMod;
        _command = command;
        _gameContainer = gameContainer;
    }
    public override Task PopulateSaveRootAsync()
    {
        FourSuitRummyPlayerItem self = PlayerList!.GetSelf();
        self.AdditionalCards = _model.TempSets!.ListAllObjects();
        PlayerList.ForEach(thisPlayer =>
        {
            thisPlayer.SetList = thisPlayer.MainSets!.SavedSets();
        });
        return base.PopulateSaveRootAsync();
    }
    public override Task FinishGetSavedAsync()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.MainSets = new MainSets(thisPlayer, _gameContainer);
            int x = thisPlayer.SetList.Count;
            x.Times(items =>
            {
                SetInfo thisSet = new(_command);
                thisPlayer.MainSets.CreateNewSet(thisSet);
            });
            thisPlayer.MainSets.LoadSets(thisPlayer.SetList);
            if (thisPlayer.AdditionalCards.Count > 0)
            {
                thisPlayer.MainHandList.AddRange(thisPlayer.AdditionalCards);
                thisPlayer.AdditionalCards.Clear();
            }
        });
        SingleInfo = PlayerList.GetSelf();
        SortCards();
        SingleInfo.DoInit();
        IsLoaded = true;
        return base.FinishGetSavedAsync();
    }
    protected override async Task ComputerTurnAsync()
    {
        await Task.CompletedTask;
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        SaveRoot!.TimesReshuffled = 0;
        if (PlayerList.First().MainSets == null)
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.MainSets = new MainSets(thisPlayer, _gameContainer);
            });
        }
        return base.StartSetUpAsync(isBeginning);
    }
    protected override Task LastPartOfSetUpBeforeBindingsAsync()
    {
        SingleInfo = PlayerList!.GetSelf();
        SingleInfo.DoInit();
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.MainSets!.ClearBoard();
            thisPlayer.SetList.Clear();
            thisPlayer.AdditionalCards.Clear();
        });
        return base.LastPartOfSetUpBeforeBindingsAsync();
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "finishedsets":
                await CreateSetsAsync(content);
                await ContinueTurnAsync();
                return;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override async Task EndTurnAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        SingleInfo.MainHandList.UnhighlightObjects();
        if (CanReshuffle == false)
        {
            await EndRoundAsync();
            return;
        }
        _command.ManuelFinish = true;
        WhoTurn = await PlayerList.CalculateWhoTurnAsync();
        await StartNewTurnAsync();
    }
    public override async Task StartNewTurnAsync()
    {
        await base.StartNewTurnAsync();
        await ContinueTurnAsync();
    }
    private async Task CreateSetsAsync(string message)
    {
        var firstTemp = await js1.DeserializeObjectAsync<BasicList<string>>(message);
        SingleInfo = PlayerList!.GetWhoPlayer();
        foreach (var thisFirst in firstTemp)
        {
            BasicList<int> thisCol = await js1.DeserializeObjectAsync<BasicList<int>>(thisFirst);
            thisCol.ForEach(deck =>
            {
                BasicList<int> otherList = SingleInfo.MainHandList.GetDeckListFromObjectList();
                string thisstr = "";
                otherList.ForEach(thisItem =>
                {
                    thisstr += thisItem + ",";
                });
                if (SingleInfo.MainHandList.ObjectExist(deck) == false)
                {
                    throw new CustomBasicException($"Deck of {deck} does not exist.  Player Is {SingleInfo.NickName}. ids are {thisstr} ");
                }
            });
            var fins = await thisFirst.GetObjectsFromDataAsync(SingleInfo.MainHandList);
            AddSet(fins);
        }
    }
    public bool CanProcessDiscard(out bool pickUp, out int index, out int deck, out string message)
    {
        message = "";
        index = 0;
        deck = 0;
        if (_gameContainer.AlreadyDrew == false && _gameContainer.PreviousCard == 0)
        {
            pickUp = true;
        }
        else
        {
            pickUp = false;
        }
        if (pickUp == true)
        {
            if (_model!.PlayerHand1!.HowManySelectedObjects > 0 || _model.TempSets!.HowManySelectedObjects > 0)
            {
                message = "There is at least one card selected.  Unselect all the cards before picking up from discard";
                return false;
            }
            return true;
        }
        var thisCol = _model!.PlayerHand1!.ListSelectedObjects();
        var otherCol = _model.TempSets!.ListSelectedObjects();
        if (thisCol.Count == 0 && otherCol.Count == 0)
        {
            message = "Sorry, you must select a card to discard";
            return false;
        }
        if (thisCol.Count + otherCol.Count > 1)
        {
            message = "Sorry, you can only select one card to discard";
            return false;
        }
        if (thisCol.Count == 0)
        {
            index = _model.TempSets.PileForSelectedObject;
            deck = _model.TempSets.DeckForSelectedObjected(index);
        }
        else
        {
            deck = _model.PlayerHand1.ObjectSelected();
        }
        var thisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
        if (thisCard.Deck == _gameContainer.PreviousCard && SingleInfo!.ObjectCount > 1)
        {
            message = "Sorry, you cannot discard the one that was picked up since there is more than one card to choose from";
            return false;
        }
        return true;
    }
    private void UnselectCards()
    {
        if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
        {
            return;
        }
        _model!.PlayerHand1!.EndTurn();
        _model.TempSets!.EndTurn();
    }
    public override async Task DiscardAsync(RegularRummyCard thisCard)
    {
        RemoveCard(thisCard.Deck);
        await AnimatePlayAsync(thisCard);
        UnselectCards();
        if (SingleInfo!.ObjectCount == 0 || Test!.EndRoundEarly)
        {
            await EndRoundAsync();
            return;
        }
        await EndTurnAsync();
    }
    private void RemoveCard(int deck)
    {
        if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Computer)
        {
            return;
        }
        if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
        {
            if (_model!.TempSets!.HasObject(deck))
            {
                _model.TempSets.RemoveObject(deck);
                return;
            }
        }
        if (SingleInfo.MainHandList.ObjectExist(deck) == false)
        {
            throw new CustomBasicException($"{deck} did not exist  count was {SingleInfo.MainHandList.Count} and the name of the player was {SingleInfo.NickName}");
        }
        SingleInfo.MainHandList.RemoveObjectByDeck(deck);
    }
    protected override Task AfterReshuffleAsync()
    {
        SaveRoot.TimesReshuffled++; //forgot this part.
        return base.AfterReshuffleAsync();
    }
    public bool CanReshuffle => SaveRoot!.TimesReshuffled < 2;
    public override async Task EndRoundAsync()
    {
        CalculateScore();
        _model!.TempSets!.ClearBoard();
        if (PlayerList.Any(items => items.TotalScore >= 1000))
        {
            SingleInfo = PlayerList.OrderByDescending(items => items.TotalScore).First();
            await ShowWinAsync();
            return;
        }
        bool rets = Aggregator.HandlerAsyncExistsFor<RoundOverEventModel>();
        await this.RoundOverNextAsync();
    }
    private void CalculateScore()
    {
        int seconds;
        int firsts;
        int scores;
        if (SingleInfo!.ObjectCount == 0)
        {
            if (WhoTurn == 1)
            {
                seconds = ScoreHand(2);
            }
            else
            {
                seconds = ScoreHand(1);
            }
            firsts = SingleInfo.MainSets!.CalculateScore;
            scores = firsts + seconds;
            SingleInfo.CurrentScore = scores;
            SingleInfo.TotalScore += scores;
            int newTurn;
            if (WhoTurn == 1)
            {
                newTurn = 2;
            }
            else
            {
                newTurn = 1;
            }
            var tempPlayer = PlayerList![newTurn];
            firsts = tempPlayer.MainSets!.CalculateScore;
            tempPlayer.CurrentScore = firsts;
            tempPlayer.TotalScore += firsts;
            return;
        }
        PlayerList!.ForEach(thisPlayer =>
        {
            firsts = thisPlayer.MainSets!.CalculateScore;
            thisPlayer.CurrentScore = firsts;
            thisPlayer.TotalScore += firsts;
        });
    }
    private static void PopulatePointsForCards(DeckRegularDict<RegularRummyCard> hand)
    {
        hand.ForEach(thisCard =>
        {
            if (thisCard.Value == EnumRegularCardValueList.HighAce)
            {
                thisCard.Points = 15;
            }
            else if (thisCard.Value >= EnumRegularCardValueList.Ten)
            {
                thisCard.Points = 10;
            }
            else
            {
                thisCard.Points = thisCard.Value.Value;
            }
        });
    }
    private DeckRegularDict<RegularRummyCard> PlayerHand()
    {
        var output = SingleInfo!.MainHandList.ToRegularDeckDict();
        if (SingleInfo.PlayerCategory != EnumPlayerCategory.Self)
        {
            PopulatePointsForCards(output);
            return output;
        }
        output.AddRange(_model!.TempSets!.ListAllObjects());
        PopulatePointsForCards(output);

        return output;
    }
    private int ScoreHand(int player)
    {
        int whos = WhoTurn;
        WhoTurn = player;
        SingleInfo = PlayerList!.GetWhoPlayer();
        var hands = PlayerHand();
        int points = hands.Sum(items => items.Points);
        WhoTurn = whos;
        SingleInfo = PlayerList.GetWhoPlayer();
        return points;
    }
    Task IStartNewGame.ResetAsync()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.TotalScore = 0;
            thisPlayer.CurrentScore = 0;
        });
        return Task.CompletedTask;
    }
    public void AddSet(IDeckDict<RegularRummyCard> thisCol)
    {
        SingleInfo!.MainSets!.AddNewSet(thisCol);
    }
    private static bool HasRun(IDeckDict<RegularRummyCard> thisCol)
    {
        if (thisCol.Count != 3)
        {
            return false; //must have 3 cards period to consider for run.
        }
        if (thisCol.DistinctCount(items => items.Suit) != 1)
        {
            return false; //must be same suit each time.
        }
        DeckRegularDict<RegularRummyCard> aceList = thisCol.Where(items => items.Value == EnumRegularCardValueList.LowAce || items.Value == EnumRegularCardValueList.HighAce).ToRegularDeckDict();
        DeckRegularDict<RegularRummyCard> tempCol = thisCol.Where(items => items.Value != EnumRegularCardValueList.LowAce
        && items.Value != EnumRegularCardValueList.HighAce).OrderBy(items => items.Value).ToRegularDeckDict();
        int x = 0;
        int previousNumber = 0;
        int currentNumber;
        int diffs;
        foreach (var thisCard in tempCol)
        {
            x++;
            if (x == 1)
            {
                previousNumber = thisCard.Value.Value;
                currentNumber = thisCard.Value.Value;
            }
            else
            {
                currentNumber = thisCard.Value.Value;
                diffs = currentNumber - previousNumber - 1;
                if (diffs > 0)
                {
                    if (aceList.Count < diffs)
                    {
                        return false;
                    }
                    for (int y = 1; y <= diffs; y++)
                    {
                        aceList.RemoveFirstItem();
                    }
                }
                previousNumber = currentNumber;
            }
        }
        return true;
    }
    public BasicList<int> SetList()
    {
        BasicList<int> output = new();
        DeckRegularDict<RegularRummyCard> thisCollection;
        IDeckDict<RegularRummyCard> tempCollection;
        for (int x = 1; x <= _model!.TempSets!.HowManySets; x++)
        {
            tempCollection = _model.TempSets.ObjectList(x);
            thisCollection = new DeckRegularDict<RegularRummyCard>();
            if (tempCollection.Count > 0)
            {
                thisCollection.AddRange(tempCollection);
            }
            if (HasRun(thisCollection))
            {
                output.Add(x);
            }
        }
        if (Test!.DoubleCheck == true && output.Count > 1)
        {
            throw new CustomBasicException("can't have more than one for now");
        }
        return output;
    }
}