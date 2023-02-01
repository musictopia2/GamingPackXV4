namespace FiveCrowns.Core.Logic;
[SingletonGame]
public class FiveCrownsMainGameClass
    : CardGameClass<FiveCrownsCardInformation, FiveCrownsPlayerItem, FiveCrownsSaveInfo>
    , IMiscDataNM, IStartNewGame, ISerializable
{
    private readonly FiveCrownsVMData _model;
    private readonly CommandContainer _command;
    private readonly FiveCrownsGameContainer _gameContainer;
    private readonly IToast _toast;
    private readonly RummyProcesses<EnumSuitList, EnumColorList, FiveCrownsCardInformation> _rummys;
    public FiveCrownsMainGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        FiveCrownsVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<FiveCrownsCardInformation> cardInfo,
        CommandContainer command,
        FiveCrownsGameContainer gameContainer,
        FiveCrownsDelegates delegates,
        ISystemError error,
        IToast toast
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, error, toast)
    {
        _model = currentMod;
        _command = command;
        _gameContainer = gameContainer;
        _toast = toast;
        _rummys = new RummyProcesses<EnumSuitList, EnumColorList, FiveCrownsCardInformation>();
        delegates.CardsToPassOut = () => CardsToPassOut;
    }
    internal int CardsToPassOut
    {
        get
        {
            return SaveRoot!.UpTo;
        }
    }
    public override Task PopulateSaveRootAsync()
    {
        SaveRoot!.SetList = _model!.MainSets!.SavedSets();
        FiveCrownsPlayerItem self = PlayerList!.GetSelf();
        self.AdditionalCards = _model.TempSets!.ListAllObjects();
        return base.PopulateSaveRootAsync();
    }
    public override Task FinishGetSavedAsync()
    {
        _model!.MainSets!.ClearBoard();
        LoadControls();
        int x = SaveRoot!.SetList.Count;
        x.Times(items =>
        {
            PhaseSet thisSet = new(_command);
            _model.MainSets.CreateNewSet(thisSet);
        });
        PlayerList!.ForEach(thisPlayer =>
        {
            if (thisPlayer.AdditionalCards.Count > 0)
            {
                thisPlayer.MainHandList.AddRange(thisPlayer.AdditionalCards);
                thisPlayer.AdditionalCards.Clear();
            }
        });
        SingleInfo = PlayerList.GetSelf();
        SortCards();
        _model.MainSets.LoadSets(SaveRoot.SetList);
        SaveRoot.LoadMod(_model);
        return base.FinishGetSavedAsync();
    }
    private void LoadControls()
    {
        if (IsLoaded == true)
        {
            return;
        }
        _rummys.HasSecond = false;
        _rummys.HasWild = true;
        _rummys.NeedMatch = true;
        _rummys.LowNumber = 3;
        _rummys.HighNumber = 13;
        IsLoaded = true;
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        if (SaveRoot!.UpTo == 0)
        {
            SaveRoot.UpTo = 3;
        }
        else
        {
            SaveRoot.UpTo++;
        }
        SaveRoot.LoadMod(_model!);
        ResetCurrentPoints();
        SaveRoot.PlayerWentOut = 0;
        _gameContainer.AlreadyDrew = false;
        SaveRoot.SetsCreated = false;
        return base.StartSetUpAsync(isBeginning);
    }
    protected override Task LastPartOfSetUpBeforeBindingsAsync()
    {
        if (IsLoaded == false)
        {
            LoadControls();
        }
        _model!.MainSets!.ClearBoard();
        SaveRoot!.SetList.Clear();
        return base.LastPartOfSetUpBeforeBindingsAsync();
    }
    private void RemoveCard(int Deck)
    {
        if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Computer)
        {
            return; //i think computer player would have already removed their card.
        }
        if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
        {
            if (_model!.TempSets!.HasObject(Deck))
            {
                _model.TempSets.RemoveObject(Deck);
                return;
            }
        }
        SingleInfo.MainHandList.RemoveObjectByDeck(Deck);
    }
    private DeckRegularDict<FiveCrownsCardInformation> PlayerHand()
    {
        var output = SingleInfo!.MainHandList.ToRegularDeckDict();
        if (SingleInfo.PlayerCategory != EnumPlayerCategory.Self)
        {
            return output;
        }
        output.AddRange(_model!.TempSets!.ListAllObjects());
        return output;
    }

    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "finishedsets":
                await CreateSetsAsync(content);
                await FinishedSetsAsync();
                return;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    private async Task CreateSetsAsync(string message)
    {
        var firstTemp = await js1.DeserializeObjectAsync<BasicList<string>>(message);
        foreach (var thisFirst in firstTemp)
        {
            var thisCol = await thisFirst.GetObjectsFromDataAsync(SingleInfo!.MainHandList);
            CreateSet(thisCol);
        }
    }
    public override async Task StartNewTurnAsync()
    {
        await base.StartNewTurnAsync();
        SaveRoot!.SetsCreated = false;
        await ContinueTurnAsync();
    }
    public override async Task EndTurnAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        UnselectCards();
        if (SaveRoot!.PlayerWentOut > 0 && SaveRoot.PlayerWentOut != WhoTurn)
        {
            var tempList = PlayerHand();
            int pointss = CalculatePoints(tempList);
            SingleInfo.CurrentScore = pointss;
            SingleInfo.TotalScore += pointss;
            _model!.TempSets!.ClearBoard();
        }
        _command.ManuelFinish = true; //because it could be somebody else's turn.
        WhoTurn = await PlayerList.CalculateWhoTurnAsync();
        if (WhoTurn == SaveRoot.PlayerWentOut || Test!.EndRoundEarly)
        {
            await EndRoundAsync();
            return;
        }
        await StartNewTurnAsync();
    }

    public void ModifyCards(IDeckDict<FiveCrownsCardInformation> thisCol)
    {
        thisCol.ForEach(thisCard =>
        {
            if (thisCard.CardValue.Value == SaveRoot!.UpTo)
            {
                thisCard.Points = 20;
            }
            else if (thisCard.CardValue == EnumCardValueList.Joker)
            {
                thisCard.Points = 50;
            }
            else
            {
                thisCard.Points = thisCard.CardValue.Value;
            }
        });
    }
    private int CalculatePoints(IDeckDict<FiveCrownsCardInformation> thisCol)
    {
        if (thisCol.Count == 0)
        {
            return 0;
        }
        ModifyCards(thisCol);
        return thisCol.Sum(items => items.Points);
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
    public IDeckDict<FiveCrownsCardInformation> WhatSet(int whichOne)
    {
        return _model!.TempSets!.ObjectList(whichOne);
    }
    public override async Task DiscardAsync(FiveCrownsCardInformation thisCard)
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        RemoveCard(thisCard.Deck);
        await AnimatePlayAsync(thisCard);
        UnselectCards();
        if (SaveRoot!.PlayerWentOut == 0 && SingleInfo.ObjectCount == 0)
        {
            SaveRoot.PlayerWentOut = WhoTurn;
            SingleInfo.CurrentScore = 0;
        }
        await EndTurnAsync();
    }
    private async Task FinishEndAsync()
    {
        if (SaveRoot!.UpTo == 13)
        {
            SingleInfo = PlayerList.OrderBy(items => items.TotalScore).First();
            await ShowWinAsync();
            return;
        }
        await this.RoundOverNextAsync();
    }
    public override async Task EndRoundAsync()
    {
        await FinishEndAsync();
    }
    public async Task FinishedSetsAsync()
    {
        SaveRoot!.SetsCreated = true;
        await ContinueTurnAsync();
    }
    public void CreateSet(IDeckDict<FiveCrownsCardInformation> thisCol)
    {
        PhaseSet thisSet = new(_command!);
        thisCol.UnhighlightObjects();
        thisSet.HandList.ReplaceRange(thisCol);
        _model!.MainSets!.CreateNewSet(thisSet);
    }
    private void ResetCurrentPoints()
    {
        PlayerList!.ForEach(thisPlayer => thisPlayer.CurrentScore = 0);
    }
    public Task ResetAsync()
    {
        PlayerList!.ForEach(thisPlayer => thisPlayer.TotalScore = 0);
        ResetCurrentPoints();
        SaveRoot!.UpTo = 0;
        return Task.CompletedTask;
    }
    private bool HasSet(IDeckDict<FiveCrownsCardInformation> thisCol)
    {
        if (thisCol.Count < 3)
        {
            return false;
        }
        if (_rummys!.IsNewRummy(thisCol, thisCol.Count, EnumRummyType.Sets))
        {
            return true;
        }
        if (_rummys.IsNewRummy(thisCol, thisCol.Count, EnumRummyType.Runs))
        {
            return true;
        }
        return false;
    }
    public bool CanProcessDiscard(out bool PickUp, out int Index, out int Deck, out string Message)
    {
        Message = "";
        Index = 0;
        Deck = 0;
        if (_gameContainer.AlreadyDrew == false && _gameContainer.PreviousCard == 0)
        {
            PickUp = true;
        }
        else
        {
            PickUp = false;
        }
        if (PickUp == true)
        {

            if (_model!.PlayerHand1!.HowManySelectedObjects > 0 || _model.TempSets!.HowManySelectedObjects > 0)
            {
                Message = "There is at least one card selected.  Unselect all the cards before picking up from discard";
                return false;
            }
            return true;
        }
        var thisCol = _model!.PlayerHand1!.ListSelectedObjects();
        var otherCol = _model.TempSets!.ListSelectedObjects();
        if (thisCol.Count == 0 && otherCol.Count == 0)
        {
            Message = "Sorry, you must select a card to discard";
            return false;
        }
        if (thisCol.Count + otherCol.Count > 1)
        {
            Message = "Sorry, you can only select one card to discard";
            return false;
        }
        if (thisCol.Count == 0)
        {
            Index = _model.TempSets.PileForSelectedObject;
            Deck = _model.TempSets.DeckForSelectedObjected(Index);
        }
        else
        {
            Deck = _model.PlayerHand1.ObjectSelected();
        }
        var thisCard = _gameContainer.DeckList!.GetSpecificItem(Deck);

        if (thisCard.Deck == _gameContainer.PreviousCard && SingleInfo!.ObjectCount > 1)
        {
            Message = "Sorry, you cannot discard the one that was picked up since there is more than one card to choose from";
            return false;
        }
        return true;
    }
    public BasicList<TempInfo> ListValidSets()
    {
        BasicList<TempInfo> output = new();
        DeckRegularDict<FiveCrownsCardInformation> thisCollection;
        IDeckDict<FiveCrownsCardInformation> tempCollection;
        TempInfo thisTemp;
        for (int x = 1; x <= 6; x++)
        {
            tempCollection = WhatSet(x);
            thisCollection = new();
            if (tempCollection.Count > 0)
            {
                thisCollection.AddRange(tempCollection);
            }
            if (thisCollection.Count > 0)
            {
                bool rets = HasSet(thisCollection);
                if (rets == true)
                {
                    thisTemp = new();
                    thisTemp.SetNumber = x;
                    thisTemp.CardList = thisCollection;
                    output.Add(thisTemp);
                }
            }
        }
        return output;
    }
    public bool CanLaterLayDown()
    {
        if (SingleInfo!.MainHandList.Count == 0)
        {
            _toast.ShowUserErrorToast("Must have one card to discard");
            return false;
        }
        return true;
    }
    public bool HasInitialSet()
    {
        if (SingleInfo!.MainHandList.Count != 1)
        {
            return false; //because must have one card to discard no matter what.
        }
        IDeckDict<FiveCrownsCardInformation> tempCollection;
        DeckRegularDict<FiveCrownsCardInformation> thisCollection;
        for (int x = 1; x <= 6; x++)
        {
            tempCollection = WhatSet(x);
            thisCollection = new DeckRegularDict<FiveCrownsCardInformation>();
            if (tempCollection.Count > 0)
            {
                thisCollection.AddRange(tempCollection);
            }
            if (thisCollection.Count > 0)
            {
                bool rets = HasSet(thisCollection);
                if (rets == false)
                {
                    return false;
                }
            }
        }
        return true;
    }
}