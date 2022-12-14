namespace BladesOfSteel.Core.Logic;
[SingletonGame]
public class BladesOfSteelMainGameClass
    : CardGameClass<RegularSimpleCard, BladesOfSteelPlayerItem, BladesOfSteelSaveInfo>
    , IMiscDataNM, ISerializable
{
    private readonly BladesOfSteelVMData _model;
    private readonly CommandContainer _command;
    private readonly BladesOfSteelGameContainer _gameContainer;
    private readonly IFaceoffProcesses _processes;
    private readonly ComputerAI _ai;
    private readonly BladesOfSteelScreenDelegates _delegates;
    private readonly IToast _toast;
    private bool _drewCard;
    private bool _firstDraw;
    public BladesOfSteelMainGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        BladesOfSteelVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<RegularSimpleCard> cardInfo,
        CommandContainer command,
        BladesOfSteelGameContainer gameContainer,
        IFaceoffProcesses processes,
        ComputerAI ai,
        BladesOfSteelScreenDelegates delegates,
        ISystemError error,
        IToast toast
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, error, toast)
    {
        _model = currentMod;
        _command = command;
        _gameContainer = gameContainer;
        _processes = processes;
        _ai = ai;
        _delegates = delegates;
        _toast = toast;
        _gameContainer.GetAttackStage = GetAttackStage;
        _gameContainer.GetDefenseStage = GetDefenseStage;
    }
    protected override bool ShowNewCardDrawn(BladesOfSteelPlayerItem tempPlayer)
    {
        return _drewCard;
    }
    protected override void SortAfterDrawing()
    {
        if (_firstDraw == false)
        {
            base.SortAfterDrawing();
            return;
        }
        SingleInfo = PlayerList!.GetSelf();
        if (SingleInfo.MainHandList.Count == 6)
        {
            SortCards();
        }
    }
    protected override async Task AfterDrawingAsync()
    {
        if (_firstDraw == false)
        {
            if (SingleInfo!.MainHandList.Count != 6)
            {
                throw new CustomBasicException("Should have 6 cards in hand after drawing");
            }
            await base.AfterDrawingAsync();
            return;
        }
        if (PlayerList.First().MainHandList.Count == 6 && PlayerList.Last().MainHandList.Count == 6)
        {
            _firstDraw = false;
        }
        await EndTurnAsync();
    }
    protected override void LinkHand()
    {
        _model!.PlayerHand1!.HandList = SingleInfo!.MainHandList; //hopefully this is it.
        PrepSort();
    }
    public override async Task DrawAsync()
    {
        if (SaveRoot!.IsFaceOff == true)
        {
            if (SingleInfo!.FaceOff != null)
            {
                throw new CustomBasicException("Player already has a faceoff card.  Therefore; cannot get another faceoff card");
            }
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.OtherHuman)
            {
                Network!.IsEnabled = true;
                return;
            }
            int deck = _gameContainer.DeckList!.ToRegularDeckDict().GetRandomItem().Deck;
            RegularSimpleCard thisCard = new();
            thisCard.Populate(deck);
            if (BasicData!.MultiPlayer == true)
            {
                await Network!.SendAllAsync("faceoff", deck);
            }
            await _processes.FaceOffCardAsync(thisCard);
            return;
        }
        await base.DrawAsync();
    }
    private async Task StartDrawingAsync()
    {
        if (SaveRoot!.IsFaceOff == true)
        {
            await ContinueTurnAsync();
            return;
        }
        SingleInfo = PlayerList!.GetWhoPlayer();
        if (_firstDraw == true)
        {
            if (SingleInfo.MainHandList.Count == 6)
            {
                throw new CustomBasicException("Cannot have 6 cards because its first draw");
            }
            LeftToDraw = 0;
            await DrawAsync();
            return;
        }
        if (SingleInfo.MainHandList.Count == 6)
        {
            await ContinueTurnAsync();
            return;
        }
        if (SingleInfo.MainHandList.Count > 6)
        {
            throw new CustomBasicException("Cannot have more than 6 cards in hand");
        }
        _drewCard = SingleInfo.MainHandList.Count > 0;
        LeftToDraw = 6 - SingleInfo.MainHandList.Count;
        if (LeftToDraw > _model!.Deck1!.CardsLeft())
        {
            await StartEndAsync();
            return;
        }
        PlayerDraws = WhoTurn;
        if (LeftToDraw == 1)
        {
            LeftToDraw = 0;
            PlayerDraws = 0;
        }
        await DrawAsync();
    }
    protected override void GetPlayerToContinueTurn()
    {
        if (SaveRoot!.PlayOrder.OtherTurn == 0)
        {
            _model!.OtherPlayer = "None";
            if (SaveRoot.IsFaceOff == true)
            {
                _model.Instructions = "Face-Off.  Click the deck to draw a card at random.  Whoever draws a higher number goes first for the game.  If there is a tie; then repeat.";
            }
            else if (_model.MainDefense1!.HasCards == true)
            {
                _model.Instructions = "Look at the results to see that the goal was blocked and end turn.";
            }
            else
            {
                _model.Instructions = "Either throw away all 6 of your cards, choose to attack or choose cards for defense.";
            }
            base.GetPlayerToContinueTurn();
            return;
        }
        SingleInfo = PlayerList!.GetOtherPlayer();
        _model!.OtherPlayer = SingleInfo.NickName;
        _model.Instructions = "Either choose cards for defense or choose to pass to allow the goal to go through";
    }
    private int CalculateWin
    {
        get
        {
            if (PlayerList.First().Score == PlayerList.Last().Score)
            {
                return 0;
            }
            if (PlayerList.First().Score > PlayerList.Last().Score)
            {
                return 1;
            }
            return 2;
        }
    }
    protected override async Task ShowHumanCanPlayAsync()
    {
        await base.ShowHumanCanPlayAsync();
        _model.YourDefensePile.ReportCanExecuteChange();
    }
    private async Task StartEndAsync()
    {
        int whoWon = CalculateWin;
        if (whoWon == 0)
        {
            _toast.ShowInfoToast("There was a tie.  Therefore; all the cards are being reshuffled.  Then a faceoff will happen again to see who goes first and the cards are passed out.  Whoever gets the first point in this round wins");
            SaveRoot!.WasTie = true;
            if (BasicData!.MultiPlayer == true && BasicData.Client == true)
            {
                Network!.IsEnabled = true;
                return;
            }
            if (_delegates.ReloadFaceoffAsync == null)
            {
                throw new CustomBasicException("Nobody is reloading the faceoff.  Rethink");
            }
            await _delegates.ReloadFaceoffAsync();
            WhoTurn = WhoStarts;
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            await SetUpGameAsync(false);
            return;
        }
        WhoTurn = whoWon;
        SingleInfo = PlayerList!.GetWhoPlayer();
        SaveRoot!.WasTie = false;
        await ShowWinAsync();
    }
    public override Task FinishGetSavedAsync()
    {
        LoadControls();
        LoadPlayerControls();
        LinkRestPiles();
        return base.FinishGetSavedAsync();
    }
    public override Task PopulateSaveRootAsync()
    {
        SaveRoot.MainDefenseList = _model.MainDefense1.GetSavedCards(); //only works if it does not have to reshuffle (i think).
        return base.PopulateSaveRootAsync();
    }
    private void LoadPlayerControls()
    {
        SaveRoot!.LoadMod(_model!);
        foreach (var player in PlayerList)
        {
            if (player.FaceOff is not null && player.FaceOff.Deck == 0)
            {
                player.FaceOff = null; //do this for now.
            }
            if (player.PlayerCategory == EnumPlayerCategory.Self)
            {
                player.DefensePile = _model.YourDefensePile;
                player.AttackPile = _model.YourAttackPile;
            }
            else
            {
                player.DefensePile = _model.OpponentDefensePile;
                player.AttackPile = _model.OpponentAttackPile;
            }
            player.DefensePile.LoadBoard(player);
            player.AttackPile.LoadBoard(player);
        }
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
            await Delay!.DelaySeconds(1);
        }
        if (SaveRoot!.IsFaceOff == true)
        {
            await DrawAsync();
            return;
        }
        if (Test.DoubleCheck == true)
        {
            return; //decided to make it stuck when double checking.  not during faceoffs though.
        }
        if (_model!.MainDefense1!.HasCards == true)
        {
            await EndTurnAsync();
            return;
        }
        if (_gameContainer.OtherTurn > 0)
        {
            var (defenseStep, cardList1) = _ai!.CardsForDefense();
            if (defenseStep == EnumDefenseStep.Pass)
            {
                await PassDefenseAsync();
                return;
            }
            if (defenseStep == EnumDefenseStep.Hand)
            {
                await PlayDefenseCardsAsync(cardList1, true);
            }
            else
            {
                await PlayDefenseCardsAsync(cardList1, false);
            }
            return;
        }
        var (firstStep, cardList2) = _ai!.GetFirstMove();
        if (firstStep == EnumFirstStep.ThrowAwayAllCards)
        {
            await ThrowAwayAllCardsFromHandAsync();
            return;
        }
        if (firstStep == EnumFirstStep.PlayDefense)
        {
            if (SingleInfo!.DefenseList.Count + cardList2.Count > 3)
            {
                await ThrowAwayDefenseCardsAsync();
                return;
            }
            await AddCardsToDefensePileAsync(cardList2);
            return;
        }
        await PlayAttackCardsAsync(cardList2);
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        if (IsLoaded == false)
        {
            LoadPlayerControls();
        }
        LoadControls();
        if (_model!.MainDefense1!.HasCards == true)
        {
            throw new CustomBasicException("Cannot start setup when there are cards in defense");
        }
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.MainHandList.Clear();
            thisPlayer.DefenseList.Clear();
            thisPlayer.AttackList.Clear();
            thisPlayer.Score = 0;
            thisPlayer.FaceOff = null;
        });
        SaveRoot.IsFaceOff = true;
        return base.StartSetUpAsync(isBeginning);
    }
    protected override Task LastPartOfSetUpBeforeBindingsAsync()
    {
        LinkRestPiles();
        _model!.Pile1!.ClearCards();
        return base.LastPartOfSetUpBeforeBindingsAsync();
    }
    private void LinkRestPiles()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.DefensePile!.HandList = thisPlayer.DefenseList;
            thisPlayer.AttackPile!.HandList = thisPlayer.AttackList;
        });
        _model.OpponentFaceOffCard.ClearCards();
        _model.YourFaceOffCard.ClearCards();
        var firstPlayer = PlayerList.GetSelf();
        if (firstPlayer.FaceOff is not null)
        {
            _model.YourFaceOffCard.AddCard(firstPlayer.FaceOff);
        }
        var secondPlayer = PlayerList.GetOnlyOpponent();
        if (secondPlayer.FaceOff is not null)
        {
            _model.OpponentFaceOffCard.AddCard(secondPlayer.FaceOff);
        }
        _firstDraw = SaveRoot.IsFaceOff;
        if (SaveRoot.MainDefenseList.Count > 0)
        {
            _model.MainDefense1.PopulateSavedCards(SaveRoot.MainDefenseList);
        }
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "passdefense":
                await PassDefenseAsync();
                return;
            case "faceoff":
                RegularSimpleCard thisCard = new();
                thisCard.Populate(int.Parse(content));
                await _processes.FaceOffCardAsync(thisCard);
                return;
            case "throwawaydefense":
                await ThrowAwayDefenseCardsAsync();
                return;
            case "throwawayallcardsfromhand":
                await ThrowAwayAllCardsFromHandAsync();
                return;
            case "defensehand":
                var thisList = await content.GetNewObjectListFromDeckListAsync(_gameContainer.DeckList!);
                await PlayDefenseCardsAsync(thisList, true);
                return;
            case "defenseboard":
                var thisList2 = await content.GetNewObjectListFromDeckListAsync(_gameContainer.DeckList!);
                await PlayDefenseCardsAsync(thisList2, false);
                return;
            case "attack":
                var thisList3 = await content.GetNewObjectListFromDeckListAsync(_gameContainer.DeckList!);
                await PlayAttackCardsAsync(thisList3);
                return;
            case "addtodefense":
                var thisList4 = await content.GetNewObjectListFromDeckListAsync(_gameContainer.DeckList!);
                await AddCardsToDefensePileAsync(thisList4);
                return;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override async Task StartNewTurnAsync()
    {
        await base.StartNewTurnAsync();
        await StartDrawingAsync();
    }
    public override async Task EndTurnAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        SingleInfo.MainHandList.UnhighlightObjects();
        _gameContainer.OtherTurn = 0;
        if (_model!.MainDefense1!.HasCards == true)
        {
            DeckRegularDict<RegularSimpleCard> thisList = _model.MainDefense1.HandList.ToRegularDeckDict();
            await thisList.ForEachAsync(async thisCard =>
            {
                _model.MainDefense1.HandList.RemoveSpecificItem(thisCard);
                await AnimatePlayAsync(thisCard);
                if (Test!.NoAnimations == false)
                {
                    await Delay!.DelaySeconds(.1);
                }
            });

            if (SingleInfo.AttackList.Count == 0)
            {
                throw new CustomBasicException("The main defense cannot have any cards because there are no cards for attack");
            }
            thisList = SingleInfo.AttackList.ToRegularDeckDict();
            await thisList.ForEachAsync(async thisCard =>
            {
                SingleInfo.AttackList.RemoveObjectByDeck(thisCard.Deck);
                await AnimatePlayAsync(thisCard);
                if (Test!.NoAnimations == false)
                {
                    await Delay!.DelaySeconds(.1);
                }
            });
            if (SingleInfo.AttackList.Count > 0)
            {
                throw new CustomBasicException("Must have 0 cards left for attack");
            }
        }
        _command.ManuelFinish = true;
        WhoTurn = await PlayerList.CalculateWhoTurnAsync();
        await StartNewTurnAsync();
    }
    #region "public functions"
    public static EnumAttackGroup GetAttackStage(IBasicList<RegularSimpleCard> thisList)
    {
        if (thisList.Any(items => items.Color == EnumRegularColorList.Black))
        {
            throw new CustomBasicException("Cannot get the attack stage if black cards are present");
        }
        if (thisList.Count < 2)
        {
            throw new CustomBasicException("Must have at least 2 cards for attack");
        }
        if (thisList.Count == 2)
        {
            if (thisList.All(items => items.Value == EnumRegularCardValueList.Nine))
            {
                return EnumAttackGroup.GreatOne;
            }
            if (thisList.HasDuplicates(items => items.Value))
            {
                return EnumAttackGroup.OneTimer;
            }
            if (thisList.Any(items => items.Value == EnumRegularCardValueList.HighAce))
            {
                bool suitMatch = thisList.HasDuplicates(items => items.Suit);
                if (suitMatch == true)
                {
                    return EnumAttackGroup.BreakAway;
                }
            }
            return EnumAttackGroup.HighCard;
        }
        if (thisList.Count > 3)
        {
            throw new CustomBasicException("Can only attack with 3 cards at the most");
        }
        int counts = thisList.DistinctCount(items => items.Suit);
        if (counts == 1)
        {
            return EnumAttackGroup.Flush;
        }
        return EnumAttackGroup.HighCard;
    }
    public static EnumDefenseGroup GetDefenseStage(IBasicList<RegularSimpleCard> thisList)
    {
        if (thisList.Any(items => items.Color == EnumRegularColorList.Red))
        {
            throw new CustomBasicException("Cannot get the defense stage if there are red cards present");
        }
        if (thisList.Count == 0)
        {
            throw new CustomBasicException("Must have at least one card for defense");
        }
        if (thisList.Count > 3)
        {
            throw new CustomBasicException("Cannot defend with more than 3 cards");
        }
        if (thisList.Count == 1)
        {
            if (thisList.Single().Value == EnumRegularCardValueList.HighAce)
            {
                return EnumDefenseGroup.StarGoalie;
            }
            return EnumDefenseGroup.HighCard;
        }
        int counts = thisList.DistinctCount(items => items.Suit);
        if (thisList.Count == 2)
        {
            if (thisList.HasDuplicates(items => items.Value) == true)
            {
                return EnumDefenseGroup.StarDefense;
            }
            return EnumDefenseGroup.HighCard;
        }
        if (counts == 1)
        {
            return EnumDefenseGroup.Flush;
        }
        return EnumDefenseGroup.HighCard;
    }
    public bool CanPlayAttackCards(IDeckDict<RegularSimpleCard> thisList)
    {
        if (thisList.Count < 2)
        {
            _toast.ShowInfoToast("Must have at least 2 cards for attacking");
            return false;
        }
        if (thisList.Any(items => items.Color == EnumRegularColorList.Black))
        {
            _toast.ShowInfoToast("Cannot attack with black (defense) cards");
            return false;
        }
        if (thisList.Count > 3)
        {
            _toast.ShowInfoToast("Cannot attack with more than 3 cards");
            return false;
        }
        return true;
    }
    public bool CanPlayDefenseCards(IDeckDict<RegularSimpleCard> thisList)
    {
        if (thisList.Count == 0)
        {
            _toast.ShowInfoToast("Must choose at least one card");
            return false;
        }
        if (thisList.Count > 3)
        {
            _toast.ShowInfoToast("Cannot play more than 3 cards for defense");
            return false;
        }
        if (thisList.Any(items => items.Color == EnumRegularColorList.Red))
        {
            _toast.ShowInfoToast("Cannot use attack (red) cards for defense");
            return false;
        }
        return true;
    }
    public async Task PlayAttackCardsAsync(IDeckDict<RegularSimpleCard> thisList)
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        SingleInfo.MainHandList.RemoveGivenList(thisList);
        thisList.UnhighlightObjects();
        if (SingleInfo.AttackList.Count > 0)
        {
            throw new CustomBasicException("There are already attack cards played.  There should not have been able to play attack cards");
        }
        SingleInfo.AttackList.AddRange(thisList);
        if (WhoTurn == 1)
        {
            _gameContainer.OtherTurn = 2;
        }
        else
        {
            _gameContainer.OtherTurn = 1;
        }
        await ContinueTurnAsync();
    }
    public async Task PlayDefenseCardsAsync(IDeckDict<RegularSimpleCard> defenseList, bool fromHand)
    {
        if (fromHand == true)
        {
            SingleInfo!.MainHandList.RemoveSelectedItems(defenseList);
        }
        else
        {
            SingleInfo!.DefenseList.RemoveSelectedItems(defenseList);
        }
        _model.MainDefense1!.PopulateObjects(defenseList);
        _gameContainer.OtherTurn = 0;
        await ContinueTurnAsync();
    }
    public async Task AddCardsToDefensePileAsync(IDeckDict<RegularSimpleCard> defenseList)
    {
        SingleInfo!.MainHandList!.RemoveGivenList(defenseList);
        SingleInfo.DefensePile!.PopulateObjects(defenseList);
        await EndTurnAsync();
    }
    public async Task PassDefenseAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        _toast.ShowInfoToast($"{SingleInfo.NickName} has scored a goal");
        SingleInfo.Score++;
        SingleInfo.AttackList.Clear();
        if (SaveRoot!.WasTie == true)
        {
            await ShowWinAsync(); //this one won period now.
            return;
        }
        await EndTurnAsync();
    }
    public async Task ThrowAwayDefenseCardsAsync()
    {
        if (SingleInfo!.DefenseList.Count == 0)
        {
            throw new CustomBasicException("There are no defense cards to throw away");
        }
        var thisList = SingleInfo.DefenseList.ToRegularDeckDict();
        await thisList.ForEachAsync(async thisCard =>
        {
            SingleInfo.DefenseList.RemoveObjectByDeck(thisCard.Deck);
            await AnimatePlayAsync(thisCard);
            if (Test!.NoAnimations == false)
            {
                await Delay!.DelaySeconds(.1);
            }
        });
        if (SingleInfo.DefenseList.Count > 0)
        {
            throw new CustomBasicException("All the defense cards should be gone");
        }
        await EndTurnAsync();
    }
    public async Task ThrowAwayAllCardsFromHandAsync()
    {
        var thisList = SingleInfo!.MainHandList.ToRegularDeckDict();
        await thisList.ForEachAsync(async thisCard =>
        {
            SingleInfo.MainHandList.RemoveObjectByDeck(thisCard.Deck);
            await AnimatePlayAsync(thisCard);
            if (Test!.NoAnimations == false)
            {
                await Delay!.DelaySeconds(.1);
            }
        });
        if (SingleInfo.MainHandList.Count > 0)
        {
            throw new CustomBasicException("Should have 0 cards left after discarding all your cards");
        }
        await EndTurnAsync();
    }
    #endregion
}