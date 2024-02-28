namespace MonasteryCardGame.Core.Logic;
[SingletonGame]
public class MonasteryCardGameMainGameClass
    : CardGameClass<MonasteryCardInfo, MonasteryCardGamePlayerItem, MonasteryCardGameSaveInfo>
    , IMiscDataNM, IStartNewGame, ISerializable
{
    private readonly MonasteryCardGameVMData _model;
    private readonly CommandContainer _command;
    private readonly MonasteryCardGameGameContainer _gameContainer;
    private readonly PrivateAutoResumeProcesses _privateAutoResume;
    internal MissionList? CurrentMission { get; set; }
    internal BasicList<MissionList>? MissionInfo { get; set; }
#pragma warning disable IDE0290 // Use primary constructor
    public MonasteryCardGameMainGameClass(IGamePackageResolver mainContainer,
#pragma warning restore IDE0290 // Use primary constructor
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        MonasteryCardGameVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<MonasteryCardInfo> cardInfo,
        CommandContainer command,
        MonasteryCardGameGameContainer gameContainer,
        ISystemError error,
        IToast toast,
        PrivateAutoResumeProcesses privateAutoResume
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, error, toast)
    {
        _model = currentMod;
        _command = command;
        _gameContainer = gameContainer;
        _privateAutoResume = privateAutoResume;
    }
    public override async Task FinishGetSavedAsync()
    {
        _model!.MainSets!.ClearBoard();
        _gameContainer.TempSets.Clear();
        LoadControls();
        int x = SaveRoot!.SetList.Count;
        x.Times(items =>
        {
            RummySet thisSet = new(_gameContainer);
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
        SingleInfo = PlayerList.GetWhoPlayer();
        if (SaveRoot.Mission == 0)
        {
            CurrentMission = null;
        }
        else
        {
            CurrentMission = MissionInfo![SaveRoot.Mission - 1];
        }
        PopulateMissions();
        bool rets;
        rets = await _privateAutoResume.HasAutoResumeAsync(_gameContainer);
        if (rets)
        {
            await _privateAutoResume.RestoreStateAsync(_gameContainer);
        }
        await base.FinishGetSavedAsync();
        CreateRummys();
    }
    private void LoadControls()
    {
        if (IsLoaded == true)
        {
            return;
        }
        CreateSets();
        IsLoaded = true;
    }
    private void CreateRummys()
    {
        if (_gameContainer.Rummys != null)
        {
            return;
        }
        _gameContainer.Rummys = new RummyClass(_gameContainer);
    }
    private void CreateSets()
    {
        MissionInfo = [];
        MissionList thisMission;
        thisMission = new();
        thisMission.Description = "2 sets of 3 in color";
        SetInfo newSets;
        int x;
        for (x = 1; x <= 2; x++)
        {
            newSets = new();
            newSets.HowMany = 3;
            newSets.SetType = EnumMonasterySets.KindColor;
            thisMission.MissionSets.Add(newSets);
        }
        MissionInfo.Add(thisMission);
        thisMission = new();
        thisMission.Description = "3 sets of 3";
        for (x = 1; x <= 3; x++)
        {
            newSets = new();
            newSets.HowMany = 3;
            newSets.SetType = EnumMonasterySets.RegularKinds;
            thisMission.MissionSets.Add(newSets);
        }
        MissionInfo.Add(thisMission);
        thisMission = new();
        thisMission.Description = "1 set of 4, 1 run of 4";
        for (x = 1; x <= 2; x++)
        {
            newSets = new SetInfo();
            if (x == 2)
            {
                newSets.HowMany = 4;
                newSets.SetType = EnumMonasterySets.RegularKinds;
            }
            else
            {
                newSets.HowMany = 4;
                newSets.SetType = EnumMonasterySets.RegularRuns;
            }
            thisMission.MissionSets.Add(newSets);
        }
        MissionInfo.Add(thisMission);
        for (x = 5; x <= 8; x++)
        {
            if (x != 7)
            {
                thisMission = new MissionList();
                newSets = new SetInfo();
                if (x == 5)
                {
                    thisMission.Description = "1 run of 5 in suit";
                    newSets.SetType = EnumMonasterySets.SuitRuns;
                }
                else if (x == 6)
                {
                    thisMission.Description = "1 run of 6 in color";
                    newSets.SetType = EnumMonasterySets.RunColors;
                }
                else
                {
                    thisMission.Description = "1 run of 8";
                    newSets.SetType = EnumMonasterySets.RegularRuns;
                }
                newSets.HowMany = x;
                thisMission.MissionSets.Add(newSets);
                MissionInfo.Add(thisMission);
            }
        }
        thisMission = new();
        thisMission.Description = "1 double run of three";
        newSets = new();
        newSets.HowMany = 3;
        newSets.SetType = EnumMonasterySets.DoubleRun;
        thisMission.MissionSets.Add(newSets);
        MissionInfo.Add(thisMission);
        thisMission = new();
        thisMission.Description = "7 cards of the same suit";
        newSets = new();
        newSets.HowMany = 7;
        newSets.SetType = EnumMonasterySets.RegularSuits;
        thisMission.MissionSets.Add(newSets);
        MissionInfo.Add(thisMission);
        thisMission = new();
        thisMission.Description = "9 cards even or odd (all)";
        newSets = new();
        newSets.HowMany = 9;
        newSets.SetType = EnumMonasterySets.EvenOdd;
        thisMission.MissionSets.Add(newSets);
        MissionInfo.Add(thisMission);
    }
    public override Task PopulateSaveRootAsync()
    {
        SaveRoot!.SetList = _model!.MainSets!.SavedSets();
        MonasteryCardGamePlayerItem self = PlayerList!.GetSelf();
        self.AdditionalCards = _model.TempSets!.ListAllObjects();
        return base.PopulateSaveRootAsync();
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        if (isBeginning)
        {
            PlayerList!.ForEach(thisPlayer => thisPlayer.UpdateIndexes());
        }
        PlayerList!.ForEach(thisPlayer => thisPlayer.FinishedCurrentMission = false);
        return base.StartSetUpAsync(isBeginning);
    }
    protected override Task LastPartOfSetUpBeforeBindingsAsync()
    {
        _gameContainer.TempSets.Clear();
        if (IsLoaded == false)
        {
            LoadControls();
            CreateRummys();
            SingleInfo = PlayerList!.GetSelf();
        }
        _model!.MainSets!.ClearBoard();
        SaveRoot!.SetList.Clear();
        SingleInfo = PlayerList!.GetWhoPlayer();
        PopulateMissions();
        return base.LastPartOfSetUpBeforeBindingsAsync();
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "finished":
                await CreateSetsAsync(content);
                if (Test!.NoAnimations == false)
                {
                    await Delay!.DelayMilli(500);
                }
                await FinishedAsync();
                return;
            case "expandset":
                var tempData = await js1.DeserializeObjectAsync<SendExpandSet>(content);
                var tempList = await tempData.CardData.GetObjectsFromDataAsync(SingleInfo!.MainHandList);
                await ExpandSetAsync(tempList, tempData.SetNumber, tempData.Position);
                return;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override async Task StartNewTurnAsync()
    {
        await base.StartNewTurnAsync();
        PopulateMissions();
        if (SingleInfo!.ObjectCount == 8)
        {
            throw new CustomBasicException("Its impossible to have only 8 cards at the start of your turn");
        }
        await ContinueTurnAsync();
    }
    public override async Task EndTurnAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        UnselectCards();
        _command.ManuelFinish = true; //because it could be somebody else's turn.
        WhoTurn = await PlayerList.CalculateWhoTurnAsync();
        await StartNewTurnAsync();
    }
    private async Task CreateSetsAsync(string message)
    {
        var firstTemp = await js1.DeserializeObjectAsync<BasicList<string>>(message);
        int x = 0;
        foreach (var thisFirst in firstTemp)
        {
            x++;
            var thisSend = await js1.DeserializeObjectAsync<SendNewSet>(thisFirst);
            var thisCol = await thisSend.CardData.GetObjectsFromDataAsync(SingleInfo!.MainHandList);
            if (x == 1)
            {
                _model!.MissionChosen = thisSend.MissionCompleted;
                ProcessCurrentMission();
            }
            CreateNewSet(thisCol, thisSend.Index);
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
    private void RemoveCard(int deck)
    {
        if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Computer)
        {
            return; //i think computer player would have already removed their card.
        }
        if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
        {
            if (_model!.TempSets!.HasObject(deck))
            {
                _model.TempSets.RemoveObject(deck);
                return;
            }
        }
        SingleInfo.MainHandList.RemoveObjectByDeck(deck);
    }
    private void RemoveCards(DeckRegularDict<MonasteryCardInfo> thisCol)
    {
        if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
        {
            return;
        }
        DeckRegularDict<MonasteryCardInfo> output = [];
        thisCol.ForEach(thisCard =>
        {
            if (_model!.TempSets!.HasObject(thisCard.Deck))
            {
                _model.TempSets.RemoveObject(thisCard.Deck);
            }
            else
            {
                output.Add(thisCard);
            }
        });
        SingleInfo.MainHandList.RemoveGivenList(output);
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
    private bool CanEndGame()
    {
        if (PlayerList.Any(items => items.IndexList.Count == 0) == false)
        {
            return false; //because nobody completed all the missions.
        }
        if (PlayerList.Count(items => items.IndexList.Count == 0) > 1)
        {
            throw new CustomBasicException("It should be impossible for both players to complete all 9 missions.");
        }
        SingleInfo = PlayerList.Single(items => items.IndexList.Count == 0);
        return true;
    }
    private int GetMissionNumber => MissionInfo!.IndexOf(CurrentMission!);
    private bool DidAllPlayersComplete => PlayerList.All(items => items.FinishedCurrentMission == true);
    public MissionList GetMission => MissionInfo!.Single(items => items.Description == _model!.MissionChosen);
    public void ProcessCurrentMission()
    {
        CurrentMission = GetMission;
        int nums = GetMissionNumber;
        SingleInfo!.CompleteMissionIndex(nums);
        _model!.CompleteMissions.ReplaceAllWithGivenItem(CurrentMission);
    }
    public void CreateNewSet(DeckRegularDict<MonasteryCardInfo> thisCol, int whichOne)
    {
        if (thisCol.Count == 0)
        {
            throw new CustomBasicException("Did not send in any cards for the set");
        }
        var thisSet = CurrentMission!.MissionSets[whichOne - 1]; //because 0 based.
        var newCol = thisSet.SetType switch
        {
            EnumMonasterySets.RegularSuits => thisCol, //because no change is needed in this situation.
            EnumMonasterySets.RegularKinds => _gameContainer.Rummys!.KindList(thisCol, false),
            EnumMonasterySets.RegularRuns => _gameContainer.Rummys!.RunList(thisCol, EnumRunType.None),
            EnumMonasterySets.SuitRuns => _gameContainer.Rummys!.RunList(thisCol, EnumRunType.Suit),
            EnumMonasterySets.DoubleRun => RummyClass.DoubleRunList(thisCol),
            EnumMonasterySets.KindColor => _gameContainer.Rummys!.KindList(thisCol, true),
            EnumMonasterySets.RunColors => _gameContainer.Rummys!.RunList(thisCol, EnumRunType.Color),
            EnumMonasterySets.EvenOdd => _gameContainer.Rummys!.EvenOddList(thisCol),
            _ => throw new CustomBasicException("None"),
        };
        RummySet thisRummy = new(_gameContainer);
        thisRummy.CreateSet(newCol, thisSet.SetType);
        if (thisRummy.HandList.Count != thisCol.Count)
        {
            throw new CustomBasicException("The rummy hand list don't match the list being sent in");
        }
        if (newCol.Count == 0)
        {
            throw new CustomBasicException("There was no list to even create a set with.");
        }
        _model!.MainSets!.CreateNewSet(thisRummy);
    }
    public async Task ExpandSetAsync(DeckRegularDict<MonasteryCardInfo> thisCol, int whichOne, int position)
    {
        var thisRummy = _model!.MainSets!.GetIndividualSet(whichOne);
        RemoveCards(thisCol);
        thisCol.ForEach(thisCard =>
        {
            thisCard.IsSelected = false;
            thisCard.Drew = false;
            thisRummy.AddCard(thisCard, position);
        });
        await FinishedAsync();
    }
    public async Task FinishedAsync()
    {
        if (SingleInfo!.ObjectCount == 0 || BasicData.MultiPlayer == false)
        {
            await EndRoundAsync();
            return;
        }
        if (CanEndGame())
        {
            await ShowWinAsync();
            return;
        }
        if (DidAllPlayersComplete)
        {
            await EndRoundAsync();
            return;
        }
        await ContinueTurnAsync();
    }
    public bool IsCardSelected()
    {
        return _model!.PlayerHand1!.HowManySelectedObjects > 0 || _model.TempSets!.HowManySelectedObjects > 0;
    }
    private void ResetSuccess()
    {
        MissionInfo!.ForEach(thisMission =>
        {
            thisMission.MissionSets.ForEach(thisSet => thisSet.DidSucceed = false);
        });
    }
    private void PopulateMissions()
    {
        var tempList = SingleInfo!.IndexList.ToBasicList();
        BasicList<MissionList> otherList = [];
        tempList.ForEach(thisIndex =>
        {
            otherList.Add(MissionInfo![thisIndex]);
        });
        _model!.PopulateMissions(otherList);
    }
    public bool DidCompleteMission(out BasicList<InstructionInfo> tempList)
    {
        tempList = [];
        var thisMission = GetMission;
        DeckRegularDict<MonasteryCardInfo> thisCollection;
        IDeckDict<MonasteryCardInfo> tempCollection;
        for (int x = 1; x <= _model!.TempSets!.HowManySets; x++)
        {
            tempCollection = _model.TempSets.ObjectList(x);
#pragma warning disable IDE0028 // Simplify collection initialization  too difficult to read though.
            thisCollection = new DeckRegularDict<MonasteryCardInfo>();
#pragma warning restore IDE0028 // Simplify collection initialization
            thisCollection.AddRange(tempCollection);
            for (int y = 1; y <= thisMission.MissionSets.Count; y++)
            {
                var newSet = thisMission.MissionSets[y - 1]; //because 0 based.

                if (newSet.DidSucceed == false)
                {
                    newSet.DidSucceed = newSet.SetType switch
                    {
                        EnumMonasterySets.RegularSuits => RummyClass.IsSuit(thisCollection, newSet.HowMany),
                        EnumMonasterySets.RegularKinds => RummyClass.IsKind(thisCollection, false, newSet.HowMany),
                        EnumMonasterySets.RegularRuns => RummyClass.IsRun(thisCollection, EnumRunType.None, newSet.HowMany),
                        EnumMonasterySets.SuitRuns => RummyClass.IsRun(thisCollection, EnumRunType.Suit, newSet.HowMany),
                        EnumMonasterySets.DoubleRun => RummyClass.IsDoubleRun(thisCollection),
                        EnumMonasterySets.KindColor => RummyClass.IsKind(thisCollection, true, newSet.HowMany),
                        EnumMonasterySets.RunColors => RummyClass.IsRun(thisCollection, EnumRunType.Color, newSet.HowMany),
                        EnumMonasterySets.EvenOdd => RummyClass.IsEvenOdd(thisCollection),
                        _ => throw new CustomBasicException("None"),
                    };
                    if (newSet.DidSucceed)
                    {
                        InstructionInfo thisInfo = new();
                        thisInfo.WhichOne = y;
                        thisInfo.SetNumber = x;
                        tempList.Add(thisInfo);
                        break;
                    }
                }
            }
            if (tempList.Count == thisMission.MissionSets.Count)
            {
                break; //so no possibility of allow more than was allowed.
            }
        }
        if (thisMission.MissionSets.Any(items => items.DidSucceed == false))
        {
            ResetSuccess();
            return false;
        }
        ResetSuccess();
        return true;
    }
    public override async Task DiscardAsync(MonasteryCardInfo thisCard)
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        RemoveCard(thisCard.Deck);
        await AnimatePlayAsync(thisCard);
        UnselectCards();
        if (SingleInfo.ObjectCount == 8)
        {
            throw new CustomBasicException("Cannot have only 8 cards after discarding");
        }
        if (SingleInfo.ObjectCount == 0)
        {
            await EndRoundAsync();
            return;
        }
        await EndTurnAsync();
    }

    public override async Task EndRoundAsync()
    {
        if (CanEndGame())
        {
            await ShowWinAsync();
            return;
        }
        await this.RoundOverNextAsync();
    }
    private void SetPlayerMissions()
    {
        PlayerList!.ForEach(thisPlayer => thisPlayer.UpdateIndexes());
    }
    Task IStartNewGame.ResetAsync()
    {
        SetPlayerMissions();
        return Task.CompletedTask;
    }
}