using BasicGameFrameworkLibrary.Blazor.LocalStorageClasses;
using BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfacesForHelpers;
namespace Phase10.Core.Logic;
[SingletonGame]
public class Phase10MainGameClass
    : CardGameClass<Phase10CardInformation, Phase10PlayerItem, Phase10SaveInfo>
    , IMiscDataNM, IStartNewGame, IMissTurnClass<Phase10PlayerItem>, ISerializable
{
    private readonly Phase10VMData _model;
    private readonly CommandContainer _command;
    private readonly Phase10GameContainer _gameContainer;
    private readonly PrivateAutoResumeProcesses _privateAutoResume;
    private readonly RummyProcesses<EnumColorTypes, EnumColorTypes, Phase10CardInformation> _rummys;
    private BasicList<PhaseList>? _phaseInfo;
#pragma warning disable IDE0290 // Use primary constructor cannot this time because it removes the intellisense this time.
    public Phase10MainGameClass(IGamePackageResolver mainContainer,
#pragma warning restore IDE0290 // Use primary constructor
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        Phase10VMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<Phase10CardInformation> cardInfo,
        CommandContainer command,
        Phase10GameContainer gameContainer,
        ISystemError error,
        IToast toast,
        PrivateAutoResumeProcesses privateAutoResume
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, error, toast)
    {
        _model = currentMod;
        _command = command;
        _gameContainer = gameContainer;
        _privateAutoResume = privateAutoResume;
        _rummys = new();
    }
    public override async Task FinishGetSavedAsync()
    {
        _model!.MainSets!.ClearBoard();
        _gameContainer.TempSets.Clear();
        LoadControls();
        int x = SaveRoot!.SetList.Count;
        x.Times(Items =>
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
        SingleInfo.MainHandList.Sort();
        _model.MainSets.LoadSets(SaveRoot.SetList);
        bool rets;
        rets = await _privateAutoResume.HasAutoResumeAsync(_gameContainer);
        if (rets)
        {
            await _privateAutoResume.RestoreStateAsync(_gameContainer);
        }
        if (SaveRoot.ImmediatelyStartTurn == false)
        {
            PrepStartTurn();
        }
        await base.FinishGetSavedAsync();
    }
    protected override void PrepStartTurn()
    {
        base.PrepStartTurn();
        _model!.CurrentPhase = _phaseInfo![SingleInfo!.Phase - 1].Description; //0 based.
    }
    public override async Task PopulateSaveRootAsync()
    {
        SaveRoot!.SetList = _model!.MainSets!.SavedSets();
        Phase10PlayerItem Self = PlayerList!.GetSelf();
        Self.AdditionalCards = _model.TempSets!.ListAllObjects();
        await base.PopulateSaveRootAsync();
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
    private void CreateSets()
    {
        _rummys.HasSecond = false;
        _rummys.HasWild = true;
        _rummys.LowNumber = 1;
        _rummys.HighNumber = 12;
        _rummys.NeedMatch = false;
        PhaseList thisPhase = new();
        thisPhase.Description = "2 Sets of 3";
        SetInfo newSets;
        _phaseInfo = [];
        2.Times(x =>
        {
            newSets = new();
            newSets.HowMany = 3;
            newSets.SetType = EnumPhase10Sets.Kinds;
            thisPhase.PhaseSets.Add(newSets);
        });
        _phaseInfo.Add(thisPhase);
        thisPhase = new();
        thisPhase.Description = "1 Set of 3, 1 Run of 4";
        2.Times(x =>
        {
            newSets = new();
            if (x == 2)
            {
                newSets.HowMany = 3;
                newSets.SetType = EnumPhase10Sets.Kinds;
            }
            else
            {
                newSets.HowMany = 4;
                newSets.SetType = EnumPhase10Sets.Runs;
            }
            thisPhase.PhaseSets.Add(newSets);
        });
        _phaseInfo.Add(thisPhase);
        thisPhase = new();
        thisPhase.Description = "1 Set of 4, 1 Run of 4";
        2.Times(x =>
        {
            newSets = new();
            newSets.HowMany = 4;
            if (x == 2)
            {
                newSets.SetType = EnumPhase10Sets.Kinds;
            }
            else
            {
                newSets.SetType = EnumPhase10Sets.Runs;
            }
            thisPhase.PhaseSets.Add(newSets);
        });
        _phaseInfo.Add(thisPhase);
        for (int x = 7; x <= 9; x++)
        {
            thisPhase = new();
            newSets = new();
            thisPhase.Description = "1 Run of " + x;
            newSets.SetType = EnumPhase10Sets.Runs;
            newSets.HowMany = x;
            thisPhase.PhaseSets.Add(newSets);
            _phaseInfo.Add(thisPhase);
        }
        thisPhase = new();
        thisPhase.Description = "2 Sets Of 4";
        for (int x = 1; x <= 2; x++)
        {
            newSets = new();
            newSets.HowMany = 4;
            newSets.SetType = EnumPhase10Sets.Kinds;
            thisPhase.PhaseSets.Add(newSets);
        }
        _phaseInfo.Add(thisPhase);
        thisPhase = new();
        thisPhase.Description = "7 Cards Of 1 Color";
        newSets = new();
        newSets.HowMany = 7;
        newSets.SetType = EnumPhase10Sets.Colors;
        thisPhase.PhaseSets.Add(newSets);
        _phaseInfo.Add(thisPhase);
        int Y;
        for (int x = 2; x <= 3; x++)
        {
            thisPhase = new();
            thisPhase.Description = "1 Set Of 5, 1 Set Of " + x;
            for (Y = 1; Y <= 2; Y++)
            {
                newSets = new();
                newSets.SetType = EnumPhase10Sets.Kinds;
                if (Y == 2)
                {
                    newSets.HowMany = x;
                }
                else
                {
                    newSets.HowMany = 5;
                }
                thisPhase.PhaseSets.Add(newSets);
            }
            _phaseInfo.Add(thisPhase);
        }
    }
    protected override async Task ComputerTurnAsync()
    {
        await Task.CompletedTask;
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        SaveRoot!.ImmediatelyStartTurn = true;
        PlayerList!.ForEach(player =>
        {
            player.MissNextTurn = false;
            player.Completed = false;
        });
        return base.StartSetUpAsync(isBeginning);
    }
    protected override async Task LastPartOfSetUpBeforeBindingsAsync()
    {
        if (IsLoaded == false)
        {
            LoadControls();
        }
        _model!.MainSets!.ClearBoard();
        _gameContainer.TempSets.Clear(); //i think.
        SaveRoot!.IsTie = false;
        SaveRoot.Skips = false;
        SaveRoot.SetList.Clear();
        Phase10CardInformation thisCard = _model.Pile1!.GetCardInfo();
        if (thisCard.CardCategory == EnumCardCategory.Skip)
        {
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync(true);
        }
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "phasecompleted":
                await CreateSetsAsync(content);
                await ProcessCompletedPhaseAsync();
                break;
            case "playerskipped":
                _model.PlayerPicker.ShowOnlyOneSelectedItem(content);
                _command.UpdateAll();
                if (_gameContainer.Test.NoAnimations == false)
                {
                    await _gameContainer.Delay.DelaySeconds(1);
                }
                await SkipPlayerAsync(content); //hopefully this simple.
                break;
            case "expandrummy":
                SendExpandedSet Expands = await js1.DeserializeObjectAsync<SendExpandedSet>(content);
                await ExpandHumanRummyAsync(Expands.Number, Expands.Deck, Expands.Position);
                break;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override async Task StartNewTurnAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        await base.StartNewTurnAsync();
        _model!.CurrentPhase = _phaseInfo![SingleInfo.Phase - 1].Description; //0 based.
        SaveRoot!.CompletedPhase = SingleInfo.Completed;
        SaveRoot.Skips = false;
        await ContinueTurnAsync();
    }
    public override async Task EndTurnAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        SingleInfo.MainHandList.UnhighlightObjects();
        SingleInfo = PlayerList!.GetWhoPlayer();
        if (SingleInfo.ObjectCount == 9)
        {
            throw new CustomBasicException("After ending turn, a player should never have only 9 cards left");
        }
        if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
        {
            UnselectCards();
        }
        _command.ManuelFinish = true; //because it could be somebody else's turn.
        WhoTurn = await PlayerList.CalculateWhoTurnAsync(true);
        await StartNewTurnAsync();
    }
    public override async Task EndRoundAsync()
    {
        if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
        {
            UnselectCards();
        }
        SaveRoot.Skips = false;
        SingleInfo = PlayerList!.GetSelf();
        var tempCol = _model!.TempSets!.ListObjectsRemoved();
        SingleInfo.MainHandList.AddRange(tempCol);
        SingleInfo.MainHandList.Sort();
        int scores;
        PlayerList.ForEach(tempPlayer =>
        {
            scores = tempPlayer.MainHandList.TotalPoints;
            tempPlayer.TotalScore += scores;
            if (tempPlayer.Completed == true)
            {
                tempPlayer.Phase++;
            }
        });
        if (CanEndGame == false)
        {
            await this.RoundOverNextAsync();
            return;
        }
        GetWinPlayer();
        if (SingleInfo == null)
        {
            await this.RoundOverNextAsync();
            if (BasicData!.MultiPlayer == false || BasicData.Client == false)
            {
                _model.Status = "There was a tie.  Therefore another round for the players who tied is needed to determine the winner";
            }
            return;
        }
        await ShowWinAsync();
    }
    private async Task CreateSetsAsync(string message)
    {
        var firstTemp = await js1.DeserializeObjectAsync<BasicList<string>>(message);
        foreach (var thisFirst in firstTemp)
        {
            var thisSend = await js1.DeserializeObjectAsync<SendNewSet>(thisFirst);
            var thisCol = await thisSend.CardListData.GetObjectsFromDataAsync(SingleInfo!.MainHandList);
            TempInfo thisTemp = new();
            thisTemp.CardList = thisCol;
            thisTemp.WhatSet = thisSend.WhatSet;
            CreateNewSet(thisTemp);
        }
    }
    Task IStartNewGame.ResetAsync()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.Phase = 1;
            thisPlayer.TotalScore = 0;
        });
        return Task.CompletedTask;
    }
    public override Task ContinueTurnAsync()
    {
        if (SingleInfo!.ObjectCount == 9)
        {
            throw new CustomBasicException("You should never have only 9 cards");
        }
        if (SaveRoot.Skips == true)
        {
            var list = PossibleSkipList;
            _model.PlayerPicker.LoadTextList(list);
            _model.PlayerPicker.UnselectAll();
            _model.PlayerChosen = "";
        }
        return base.ContinueTurnAsync();
    }
    private void RemoveCard(int deck)
    {
        if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Computer)
        {
            throw new CustomBasicException("Computer should have never gone on this game.");
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
    public bool IsCardSelected()
    {
        if (SingleInfo!.MainHandList.HowManySelectedItems > 0)
        {
            return true;
        }
        return _model!.TempSets!.HowManySelectedObjects > 0;
    }
    public IDeckDict<Phase10CardInformation> WhatSet(int whichOne)
    {
        return _model!.TempSets!.ObjectList(whichOne);
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
    private bool CanEndGame
    {
        get
        {
            if (SaveRoot!.IsTie == true)
            {
                return true;
            }
            return PlayerList.Any(items => items.Phase == 11);
        }
    }
    private BasicList<string> PossibleSkipList
    {
        get
        {
            return PlayerList!.Where(xx => xx.MissNextTurn == false && xx.Id != WhoTurn).Select(xx => xx.NickName).ToBasicList();
        }
    }
    public override async Task DiscardAsync(Phase10CardInformation thisCard)
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        RemoveCard(thisCard.Deck);
        await AnimatePlayAsync(thisCard);
        if (SingleInfo.ObjectCount == 0)
        {
            await EndRoundAsync();
            return;
        }
        if (thisCard.CardCategory == EnumCardCategory.Skip)
        {
            await ChooseSkipAsync();
        }
        else
        {
            await EndTurnAsync();
        }
    }
    private async Task ChooseSkipAsync()
    {
        var list = PossibleSkipList;
        if (list.Count == 0)
        {
            await EndTurnAsync();
            return;
        }
        if (list.Count == 1)
        {
            await SkipPlayerAsync(list.Single());
            return;
        }
        SaveRoot!.Skips = true; //means you have to choose someone to skip
        _command.ManuelFinish = true;
        _model.PlayerChosen = "";
        await ContinueTurnAsync();
        //_toast.ShowUserErrorToast("Needs To Think About Skipping Player");
    }
    public async Task SkipPlayerAsync(string nickName)
    {
        PlayerList![nickName].MissNextTurn = true;
        await EndTurnAsync();
    }
    private void GetWinPlayer()
    {
        BasicList<Phase10PlayerItem> firstList = PlayerList.Where(items => items.Phase == 11).OrderBy(items => items.TotalScore).ToBasicList();
        if (firstList.Count == 0)
        {
            throw new CustomBasicException("Game Should Not Be Over");
        }
        if (firstList.Count == 1)
        {
            SingleInfo = firstList.Single();
            return;
        }
        if (firstList.First().TotalScore < firstList[1].TotalScore)
        {
            SingleInfo = firstList.First();
            return;
        }
        PlayerList!.ForEach(Items => Items.InGame = false);
        int totalScore = firstList.First().TotalScore;
        firstList.KeepConditionalItems(Items => Items.TotalScore == totalScore);
        if (firstList.Count <= 1)
        {
            throw new CustomBasicException("Should not be tie");
        }
        firstList.ForEach(Items => Items.InGame = true);
        SingleInfo = null;
    }
    Task IMissTurnClass<Phase10PlayerItem>.PlayerMissTurnAsync(Phase10PlayerItem player)
    {
        return Task.CompletedTask; //this does not do anything for missing next turn.
    }
    private void ResetSuccess()
    {
        _phaseInfo!.ForEach(thisPhase =>
        {
            thisPhase.PhaseSets.ForEach(thisSet => thisSet.DidSucceed = false);
        });
    }
    public bool DidCompletePhase(out int howMany)
    {
        int phase = SingleInfo!.Phase;
        howMany = 0;
        PhaseList thisPhase = _phaseInfo![phase - 1];
        DeckRegularDict<Phase10CardInformation> thisCollection;
        IDeckDict<Phase10CardInformation> tempCollection;
        Phase10CardInformation thisCard;
        for (int x = 1; x <= _model!.TempSets!.HowManySets; x++)
        {
            tempCollection = WhatSet(x);
            thisCollection = [];
            if (tempCollection.Count > 0)
            {
                thisCollection.AddRange(tempCollection);
            }
            if (thisCollection.Count > 0)
            {
                foreach (var newSet in thisPhase.PhaseSets)
                {
                    if (newSet.DidSucceed == false)
                    {
                        newSet.DidSucceed = newSet.SetType switch
                        {
                            EnumPhase10Sets.Colors => _rummys!.IsNewRummy(thisCollection, newSet.HowMany, EnumRummyType.Colors),
                            EnumPhase10Sets.Kinds => _rummys!.IsNewRummy(thisCollection, newSet.HowMany, EnumRummyType.Sets),
                            EnumPhase10Sets.Runs => _rummys!.IsNewRummy(thisCollection, newSet.HowMany, EnumRummyType.Runs),
                            _ => throw new CustomBasicException("Not Supported"),
                        };
                        if (newSet.DidSucceed == true)
                        {
                            thisCard = thisCollection.First();
                            howMany += thisCollection.Count;
                            break;
                        }
                    }
                }
            }
        }
        if (thisPhase.PhaseSets.Any(Items => Items.DidSucceed == false))
        {
            howMany = 0; //because you failed so does not matter anyway obviously.
            ResetSuccess();
            return false;
        }
        ResetSuccess();
        return true;
    }
    public BasicList<TempInfo> ListValidSets()
    {
        BasicList<TempInfo> output = [];
        int phase = SingleInfo!.Phase;
        PhaseList thisPhase = _phaseInfo![phase - 1]; //because 0 based.
        DeckRegularDict<Phase10CardInformation> thisCollection;
        IDeckDict<Phase10CardInformation> tempCollection;
        TempInfo thisTemp;
        for (int x = 1; x <= _model!.TempSets!.HowManySets; x++)
        {
            tempCollection = WhatSet(x);
            thisCollection = [];
            if (tempCollection.Count > 0)
            {
                thisCollection.AddRange(tempCollection);
            }
            if (thisCollection.Count > 0)
            {
                foreach (var newSet in thisPhase.PhaseSets)
                {
                    if (newSet.DidSucceed == false)
                    {
                        newSet.DidSucceed = newSet.SetType switch
                        {
                            EnumPhase10Sets.Colors => _rummys!.IsNewRummy(thisCollection, newSet.HowMany, EnumRummyType.Colors),
                            EnumPhase10Sets.Kinds => _rummys!.IsNewRummy(thisCollection, newSet.HowMany, EnumRummyType.Sets),
                            EnumPhase10Sets.Runs => _rummys!.IsNewRummy(thisCollection, newSet.HowMany, EnumRummyType.Runs),
                            _ => throw new CustomBasicException("Not Supported"),
                        };
                        if (newSet.DidSucceed == true)
                        {
                            thisTemp = new();
                            thisTemp.CardList = thisCollection;
                            thisTemp.WhatSet = newSet.SetType;
                            if (newSet.SetType == EnumPhase10Sets.Runs)
                            {
                                thisTemp.FirstNumber = _rummys!.FirstUsed;
                                thisTemp.SecondNumber = _rummys!.FirstUsed + thisCollection.Count - 1;
                            }
                            output.Add(thisTemp);
                            _model.TempSets.ClearBoard(x);
                            break;
                        }
                    }
                }
            }
            if (output.Count == thisPhase.PhaseSets.Count)
            {
                break;
            }
        }
        ResetSuccess();
        return output;
    }
    public async Task ProcessCompletedPhaseAsync()
    {
        SaveRoot!.CompletedPhase = true;
        SingleInfo!.Completed = true;
        await ContinueTurnAsync();
    }
    public void CreateNewSet(TempInfo thisTemp)
    {
        PhaseSet thisSet = new(_command!);
        thisSet.CreateSet(thisTemp.CardList, thisTemp.WhatSet);
        _model!.MainSets!.CreateNewSet(thisSet);
    }
    public async Task ExpandHumanRummyAsync(int phaseSet, int deck, int position)
    {
        PhaseSet thisPhase = _model!.MainSets!.GetIndividualSet(phaseSet);
        Phase10CardInformation thisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
        thisPhase.AddCard(thisCard, position);
        RemoveCard(deck);
        await ContinueTurnAsync();
    }
    public bool CanHumanExpand(PhaseSet thisPhase, ref int position, out Phase10CardInformation? whatCard, out string message)
    {
        message = "";
        whatCard = null;
        int howManySelected = _model!.PlayerHand1!.HowManySelectedObjects + _model.TempSets!.HowManySelectedObjects;
        if (howManySelected == 0)
        {
            return false;
        }
        if (howManySelected > 1)
        {
            return false;
        }
        if (_model.TempSets.TotalObjects + SingleInfo!.MainHandList.Count == 1)
        {
            return false;
        }
        int decks;
        if (_model.TempSets.HowManySelectedObjects == 1)
        {
            int piles = _model.TempSets.PileForSelectedObject;
            decks = _model.TempSets.DeckForSelectedObjected(piles);
        }
        else
        {
            decks = _model.PlayerHand1.ObjectSelected();
        }
        whatCard = _gameContainer.DeckList!.GetSpecificItem(decks);
        int Nums = thisPhase.PositionToPlay(whatCard, position);
        if (Nums == 0)
        {
            position = 0;
            whatCard = null;
            return false;
        }
        position = Nums;
        return true;
    }
    public bool CanProcessDiscard(out bool pickUp, out int index, out int deck, out string message)
    {
        index = -1; //defaults to -1
        message = "";
        deck = 0; //has to populate defaults first.
        if (_gameContainer.AlreadyDrew == false && _gameContainer.PreviousCard == 0)
        {
            pickUp = true;
        }
        else
        {
            pickUp = false;
        }
        Phase10CardInformation thisCard;
        if (pickUp == true)
        {
            if (IsCardSelected() == true)
            {
                message = "There is at least one card selected.  Unselect all the cards before picking up from discard";
                return false;
            }
            thisCard = _model!.Pile1!.GetCardInfo();
            if (thisCard.CardCategory == EnumCardCategory.Skip)
            {
                message = "Sorry, cannot pickup a skip";
                return false;
            }
            return true;
        }
        var SelectList = SingleInfo!.MainHandList.GetSelectedItems();
        int Counts = SelectList.Count + _model!.TempSets!.HowManySelectedObjects;
        if (Counts > 1)
        {
            message = "Sorry, you can only select one card to discard";
            return false;
        }
        if (Counts == 0)
        {
            message = "Sorry, you must select a card to discard";
            return false;
        }
        if (SelectList.Count == 1)
        {
            index = 0;
            deck = _model.PlayerHand1!.ObjectSelected();
        }
        else
        {
            index = _model.TempSets.PileForSelectedObject;
            deck = _model.TempSets.DeckForSelectedObjected(index);
        }
        thisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
        if (thisCard.Deck == _gameContainer.PreviousCard && SingleInfo.MainHandList.Count + _model.TempSets.TotalObjects > 1)
        {
            deck = 0;
            index = -1;
            message = "Sorry, you cannot discard the one that was picked up since there is more than one card to choose from";
            return false;
        }
        return true;
    }
}