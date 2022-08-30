namespace Rummy500.Core.Logic;
[SingletonGame]
public class Rummy500MainGameClass
    : CardGameClass<RegularRummyCard, Rummy500PlayerItem, Rummy500SaveInfo>
    , IMiscDataNM, IStartNewGame, ISerializable
{
    private readonly Rummy500VMData _model;
    private readonly CommandContainer _command;
    private readonly Rummy500GameContainer _gameContainer;
    private readonly RummyProcesses<EnumSuitList, EnumRegularColorList, RegularRummyCard> _rummys;
    public Rummy500MainGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        Rummy500VMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<RegularRummyCard> cardInfo,
        CommandContainer command,
        Rummy500GameContainer gameContainer,
        ISystemError error,
        IToast toast
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, error, toast)
    {
        _model = currentMod;
        _command = command;
        _gameContainer = gameContainer;
        _rummys = new();
    }
    public override Task FinishGetSavedAsync()
    {
        LoadControls();
        _model!.MainSets1!.ClearBoard();
        int x = SaveRoot!.SetList.Count;
        x.Times(items =>
        {
            RummySet thisSet = new(_gameContainer);
            _model.MainSets1.CreateNewSet(thisSet);
        });
        _model.MainSets1.LoadSets(SaveRoot.SetList);
        _model.DiscardList1!.HandList.ReplaceRange(SaveRoot.DiscardList);
        return base.FinishGetSavedAsync();
    }
    public override Task PopulateSaveRootAsync()
    {
        SaveRoot!.SetList = _model!.MainSets1!.SavedSets();
        SaveRoot.DiscardList = _model.DiscardList1!.HandList.ToRegularDeckDict();
        return base.PopulateSaveRootAsync();
    }
    private void LoadControls()
    {
        if (IsLoaded == true)
        {
            return;
        }
        _rummys.HasSecond = true;
        _rummys.HasWild = false;
        _rummys.LowNumber = 1;
        _rummys.HighNumber = 14;
        _rummys.NeedMatch = true;
        IsLoaded = true;
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        LoadControls();
        _model!.DiscardList1!.ClearDiscardList();
        _model.MainSets1!.ClearBoard();
        SaveRoot!.MoreThanOne = false;
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.CurrentScore = 0;
            thisPlayer.CardsPlayed = 0;
            thisPlayer.PointsPlayed = 0;
        });
        return base.StartSetUpAsync(isBeginning);
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "pickupfromdiscard":
                await PickupFromDiscardAsync(int.Parse(content));
                break;
            case "addtoset":
                SendAddSet sendAdd = await js.DeserializeObjectAsync<SendAddSet>(content);
                await AddToSetAsync(sendAdd.Index, sendAdd.Deck, sendAdd.Position);
                break;
            case "newset":
                SendNewSet sendNew = await js.DeserializeObjectAsync<SendNewSet>(content);
                var newList = sendNew.DeckList.GetNewObjectListFromDeckList(_gameContainer.DeckList!);
                await CreateNewSetAsync(newList, sendNew.SetType, sendNew.UseSecond);
                break;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
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
    public bool CanProcessDiscard(out bool pickUp, ref int deck, out string message)
    {
        message = "";
        if (_gameContainer.AlreadyDrew == false && _gameContainer.PreviousCard == 0)
        {
            pickUp = true;
        }
        else
        {
            pickUp = false;
        }
        var thisCol = _model!.PlayerHand1!.ListSelectedObjects();
        if (pickUp == true)
        {
            if (thisCol.Count > 0)
            {
                message = "There is at least one card selected.  Unselect all the cards before picking up from discard";
                return false;
            }
            return true;
        }
        if (thisCol.Count == 0)
        {
            message = "Sorry, you must select a card to discard";
            return false;
        }
        if (thisCol.Count > 1)
        {
            message = "Sorry, you can only select one card to discard";
            return false;
        }
        if (SaveRoot!.MoreThanOne == true)
        {
            if (SingleInfo!.MainHandList.Any(items => items.Deck == _gameContainer.PreviousCard))
            {
                message = "Sorry, since you picked up more than one card from the discard pile, the one clicked must be used in a set";
                return false;
            }
        }
        if (thisCol.First().Deck == _gameContainer.PreviousCard)
        {
            message = "Sorry, you cannot discard the one that was picked up since there is more than one card to choose from";
            return false;
        }
        deck = thisCol.First().Deck;
        return true;
    }
    internal bool NeedsExpansion(DeckRegularDict<RegularRummyCard> list)
    {
        if (SaveRoot.MoreThanOne == false)
        {
            return false;
        }
        if (list.Any(x => x.Deck == _gameContainer.PreviousCard))
        {
            return false;
        }
        var hand = SingleInfo!.MainHandList.ToRegularDeckDict();
        hand.RemoveGivenList(list);
        return hand.Any(x => x.Deck == _gameContainer.PreviousCard);
    }
    public override async Task DiscardAsync(RegularRummyCard thisCard)
    {
        SingleInfo!.MainHandList.RemoveObjectByDeck(thisCard.Deck);
        _model!.DiscardList1!.AddToDiscard(thisCard);
        await EndTurnAsync();
    }
    public override async Task StartNewTurnAsync()
    {
        await base.StartNewTurnAsync();
        SaveRoot!.MoreThanOne = false;
        await ContinueTurnAsync();
    }
    public override async Task EndTurnAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        SingleInfo.MainHandList.UnhighlightObjects();
        _model!.MainSets1!.EndTurn();
        _command.ManuelFinish = true; //because it could be somebody else's turn.
        if (SingleInfo.PlayerCategory != EnumPlayerCategory.Computer)
        {
            if (SingleInfo.MainHandList.Count == 0 || _model!.Deck1!.IsEndOfDeck() == true)
            {
                await EndRoundAsync();
                return;
            }
        }
        WhoTurn = await PlayerList.CalculateWhoTurnAsync();
        await StartNewTurnAsync();
    }
    protected override Task LastPartOfSetUpBeforeBindingsAsync()
    {
        _model!.DiscardList1!.AddToDiscard(_model.Deck1!.DrawCard());
        return base.LastPartOfSetUpBeforeBindingsAsync();
    }
    public override async Task EndRoundAsync()
    {
        CalculatePoints();
        if (IsGameOver() == true)
        {
            await ShowWinAsync();
            return;
        }
        await this.RoundOverNextAsync();
    }
    private void CalculatePoints()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            var thisPoint = CalculatePoints(thisPlayer.Id);
            int negs = thisPlayer.MainHandList.Sum(xx => xx.NegativePoints());
            thisPlayer.CurrentScore = thisPoint.Points + negs; //this will already be negative.
            thisPlayer.TotalScore += thisPlayer.CurrentScore;
        });
    }
    private PointInfo CalculatePoints(int player)
    {
        PointInfo output = new();
        PointInfo temps;
        _model!.MainSets1!.SetList.ForEach(thisSet =>
        {
            temps = thisSet.GetPointInfo(player);
            output.Points += temps.Points;
            output.NumberOfCards += temps.NumberOfCards;
        });
        return output;
    }
    private bool IsGameOver()
    {
        SingleInfo = PlayerList.Where(items => items.TotalScore >= 500).OrderByDescending(xx => xx.TotalScore).FirstOrDefault();
        return SingleInfo != null;
    }
    public async Task PickupFromDiscardAsync(int deck)
    {
        var thisList = _model!.DiscardList1!.DiscardListSelected(deck);
        await PickupFromDiscardAsync(thisList);
    }
    public async Task PickupFromDiscardAsync(IDeckDict<RegularRummyCard> thisCol)
    {
        var thisCard = thisCol.First();
        _gameContainer.PreviousCard = thisCard.Deck;
        _model!.DiscardList1!.RemoveFromPoint(thisCard.Deck);
        thisCol.ForEach(tempCard =>
        {
            tempCard.Drew = true;
        });
        SingleInfo!.MainHandList.AddRange(thisCol);
        SaveRoot!.MoreThanOne = thisCol.Count > 1;
        if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
        {
            SortCards();
        }
        _gameContainer.AlreadyDrew = true; //this counts as already drawing too.
        await ContinueTurnAsync();
    }
    public DeckRegularDict<RegularRummyCard> AppendDiscardList(IDeckDict<RegularRummyCard> thisCol)
    {
        DeckRegularDict<RegularRummyCard> output = new();
        output.AddRange(thisCol);
        output.AddRange(SingleInfo!.MainHandList);
        return output;
    }
    public bool CardContainsRummy(int deck, IDeckDict<RegularRummyCard> newSet)
    {
        int x;
        var thisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
        int plays;
        foreach (var thisSet in _model!.MainSets1!.SetList)
        {
            for (x = 1; x <= 2; x++)
            {
                plays = thisSet.PositionToPlay(thisCard);
                if (plays > 0)
                {
                    return true;
                }
            }
        }
        if (newSet.Count < 4)
        {
            return false; //because still needs one for discard.
        }
        int manys = newSet.Count(items => items.Value == thisCard.Value);
        if (manys >= 3)
        {
            return true; //other cases are covered.
        }
        DeckRegularDict<RegularRummyCard> mainList;
        do
        {
            mainList = newSet.ToRegularDeckDict();
            var tempCol = _rummys!.WhatNewRummy(mainList, 3, EnumRummyType.Runs, false);
            if (tempCol.Count == 0)
            {
                return false;
            }
            if (tempCol.Count == newSet.Count)
            {
                return false; //because nothing left to discard.
            }
            if (tempCol.Any(Items => Items.Deck == deck))
            {
                return true;
            }
            newSet.RemoveGivenList(tempCol);
        } while (true);
    }
    public bool IsValidRummy(IDeckDict<RegularRummyCard> thisCol, out EnumWhatSets whatType, out bool useSecond)
    {
        whatType = EnumWhatSets.kinds;
        useSecond = false;
        if (thisCol.Count < 3)
        {
            return false; //you have to have at least 3 cards for a rummy.
        }
        bool rets;
        var newList = thisCol.ToRegularDeckDict();
        rets = _rummys!.IsNewRummy(newList, thisCol.Count, EnumRummyType.Sets);
        if (rets == true)
        {
            return true;
        }
        rets = _rummys.IsNewRummy(newList, thisCol.Count, EnumRummyType.Runs);
        if (rets == true)
        {
            whatType = EnumWhatSets.runs;
            useSecond = _rummys.UseSecond;
            return true;
        }
        return false;
    }
    public async Task CreateNewSetAsync(IDeckDict<RegularRummyCard> thisCol, EnumWhatSets setType, bool useSecond)
    {
        DeckRegularDict<RegularRummyCard> newCol = new();
        thisCol.ForEach(ThisCard => newCol.Add(SingleInfo!.MainHandList.GetSpecificItem(ThisCard.Deck)));
        SingleInfo!.MainHandList.RemoveGivenList(newCol);
        RummySet thisSet = new(_gameContainer!);
        thisSet.CreateNewSet(thisCol, setType, useSecond);
        _model!.MainSets1!.CreateNewSet(thisSet);
        UpdatePoints();
        await ContinueTurnAsync();
    }
    public async Task AddToSetAsync(int whatSet, int deck, int position)
    {
        RummySet thisSet = _model!.MainSets1!.GetIndividualSet(whatSet);
        SingleInfo!.MainHandList.RemoveObjectByDeck(deck);
        RegularRummyCard thisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
        thisSet.AddCard(thisCard, position);
        UpdatePoints();
        await ContinueTurnAsync();
    }
    private void UpdatePoints()
    {
        PointInfo thisPoint = CalculatePoints(WhoTurn);
        SingleInfo!.CardsPlayed = thisPoint.NumberOfCards;
        SingleInfo.PointsPlayed = thisPoint.Points;
    }
}