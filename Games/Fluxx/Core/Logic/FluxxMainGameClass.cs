namespace Fluxx.Core.Logic;
[SingletonGame]
public class FluxxMainGameClass
    : CardGameClass<FluxxCardInformation, FluxxPlayerItem, FluxxSaveInfo>
    , IMiscDataNM, ISerializable
{
    private readonly FluxxVMData _model;
    private readonly CommandContainer _command;
    private readonly FluxxGameContainer _gameContainer;
    private readonly IGiveTaxationProcesses _giveTaxationProcesses;
    private readonly IEmptyTrashProcesses _emptyTrashProcesses;
    private readonly IPlayProcesses _playProcesses;
    private readonly ActionContainer _actionContainer;
    private readonly KeeperContainer _keeperContainer;
    private readonly FluxxDelegates _delegates;
    private readonly IShowActionProcesses _showActionProcesses;
    private readonly IDiscardProcesses _discardProcesses;
    private readonly IRotateTradeHandProcesses _rotateTradeHandProcesses;
    private readonly IFinalKeeperProcesses _finalKeeperProcesses;
    private readonly IFinalRuleProcesses _finalRuleProcesses;
    private readonly IEverybodyOneProcesses _everybodyOneProcesses;
    private readonly IDrawUseProcesses _drawUseProcesses;
    private readonly IAnalyzeProcesses _analyzeProcesses;
    private readonly ILoadActionProcesses _loadActionProcesses;
    public FluxxMainGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        FluxxVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<FluxxCardInformation> cardInfo,
        CommandContainer command,
        FluxxGameContainer gameContainer,
        IGiveTaxationProcesses giveTaxationProcesses,
        IEmptyTrashProcesses emptyTrashProcesses,
        IPlayProcesses playProcesses,
        ActionContainer actionContainer,
        KeeperContainer keeperContainer,
        FluxxDelegates delegates,
        IShowActionProcesses showActionProcesses,
        IDiscardProcesses discardProcesses,
        IRotateTradeHandProcesses rotateTradeHandProcesses,
        IFinalKeeperProcesses finalKeeperProcesses,
        IFinalRuleProcesses finalRuleProcesses,
        IEverybodyOneProcesses everybodyOneProcesses,
        IDrawUseProcesses drawUseProcesses,
        IAnalyzeProcesses analyzeProcesses,
        ILoadActionProcesses loadActionProcesses,
        ISystemError error,
        IToast toast
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, error, toast)
    {
        _model = currentMod;
        _command = command;
        _gameContainer = gameContainer;
        _giveTaxationProcesses = giveTaxationProcesses;
        _emptyTrashProcesses = emptyTrashProcesses;
        _playProcesses = playProcesses;
        _actionContainer = actionContainer;
        _keeperContainer = keeperContainer;
        _delegates = delegates;
        _showActionProcesses = showActionProcesses;
        _discardProcesses = discardProcesses;
        _rotateTradeHandProcesses = rotateTradeHandProcesses;
        _finalKeeperProcesses = finalKeeperProcesses;
        _finalRuleProcesses = finalRuleProcesses;
        _everybodyOneProcesses = everybodyOneProcesses;
        _drawUseProcesses = drawUseProcesses;
        _analyzeProcesses = analyzeProcesses;
        _loadActionProcesses = loadActionProcesses;
        _model.PlayerHand1.AutoSelect = EnumHandAutoType.SelectAsMany;
    }
    protected override void SetUpSelfHand()
    {
        _actionContainer.SetUpFrames();
        _model.Keeper1!.HandList = SingleInfo!.KeeperList;
    }
    protected override void GetPlayerToContinueTurn() { }
    public int OtherTurn
    {
        get
        {
            return SaveRoot!.PlayOrder.OtherTurn;
        }
        set
        {
            SaveRoot!.PlayOrder.OtherTurn = value;
        }
    }
    public override async Task FinishGetSavedAsync()
    {
        LoadControls();
        await base.FinishGetSavedAsync();
        SaveRoot!.LoadMod(_model!);
        LastStepAll();
        _actionContainer.LoadSavedGame(_gameContainer);
    }
    protected override Task LastPartOfSetUpBeforeBindingsAsync()
    {
        if (_model!.Deck1!.CardExists(1))
        {
            throw new CustomBasicException("The first card cannot be in the deck");
        }
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.KeeperList.Clear();
        });
        SaveRoot!.GoalList.Clear();
        SaveRoot.QueList.Clear();
        SaveRoot.SavedActionData.TempDiscardList.Clear();
        SaveRoot.SavedActionData.TempHandList.Clear();
        SaveRoot.SavedActionData.SelectedIndex = -1;
        SaveRoot.CurrentAction = 0;
        SaveRoot.RuleList.Clear();
        SaveRoot.RuleList.Add((RuleCard)_gameContainer.DeckList!.GetSpecificItem(1));
        LastStepAll();
        return base.LastPartOfSetUpBeforeBindingsAsync();
    }
    private void LoadControls()
    {
        if (IsLoaded == true)
        {
            return;
        }
        _keeperContainer.Init();
        IsLoaded = true;
    }
    protected override async Task ComputerTurnAsync()
    {
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelaySeconds(.5);
        }
        if (OtherTurn > 0)
        {
            if (_gameContainer.IsFirstPlayRandom())
            {
                await _playProcesses.PlayRandomCardAsync(ComputerAI.FirstRandomPlayed(_gameContainer));
                return;
            }
            if (_gameContainer!.CurrentAction != null && _gameContainer.CurrentAction.Deck == EnumActionMain.Taxation)
            {
                await _giveTaxationProcesses.GiveCardsForTaxationAsync(ComputerAI.CardsForTaxation(_gameContainer));
                return;
            }
            int cardsNeeded = SingleInfo!.KeeperList.Count - SaveRoot!.KeeperLimit;
            if (cardsNeeded > 0 && SaveRoot.KeeperLimit > -1)
            {
                await _discardProcesses.DiscardKeepersAsync(ComputerAI.DiscardKeepers(_gameContainer, cardsNeeded));
                return;
            }
            cardsNeeded = SingleInfo.MainHandList.Count - SaveRoot.HandLimit;
            if (cardsNeeded <= 0)
            {
                throw new CustomBasicException("Since keepers are not being discarded and no cards are being discarded; there are no limits that still needs to be obeyed");
            }
            if (SaveRoot.HandLimit > -1)
            {
                await _discardProcesses.DiscardFromHandAsync(ComputerAI.CardsToDiscardFromHand(_gameContainer, cardsNeeded));
                return;
            }
        }
        if (_delegates.CurrentScreen == null)
        {
            throw new CustomBasicException("Nobody is figuring out screen used.  Rethink");
        }
        EnumActionScreen screen = _delegates.CurrentScreen.Invoke();
        if (screen == EnumActionScreen.KeeperScreen)
        {
            if (_gameContainer!.CurrentAction!.Deck == EnumActionMain.TrashAKeeper || _gameContainer.CurrentAction.Deck == EnumActionMain.StealAKeeper)
            {
                bool isTrashed = _gameContainer.CurrentAction.Deck == EnumActionMain.TrashAKeeper;
                var thisKeeper = ComputerAI.KeeperToStealTrash(_gameContainer, isTrashed);
                _keeperContainer.ShowSelectedKeepers(new() { thisKeeper });
                await _finalKeeperProcesses.ProcessTrashStealKeeperAsync(thisKeeper, isTrashed);
                return;
            }
            if (_gameContainer.CurrentAction.Deck == EnumActionMain.ExchangeKeepers)
            {
                var keeperTuple = ComputerAI.ExchangeKeepers(_gameContainer);
                _keeperContainer.ShowSelectedKeepers(new() { keeperTuple.Item1, keeperTuple.Item2 });
                await _finalKeeperProcesses.ProcessExchangeKeepersAsync(keeperTuple.Item1, keeperTuple.Item2);
                return;
            }
            throw new CustomBasicException("The scroll keepers should not be visible for computer player.  Rethink");
        }
        int deck;
        int selectedIndex;
        BasicList<int> tempList;
        if (screen == EnumActionScreen.ActionScreen)
        {
            switch (_gameContainer!.CurrentAction!.Deck)
            {
                case EnumActionMain.Draw3Play2OfThem:
                case EnumActionMain.Draw2AndUseEm:
                    deck = ComputerAI.TempCardUse(_gameContainer);
                    await _showActionProcesses.ShowCardUseAsync(deck);
                    await _drawUseProcesses.DrawUsedAsync(deck);
                    break;
                case EnumActionMain.EverybodyGets1:
                    selectedIndex = ComputerAI.GetPlayerSelectedIndex(_actionContainer);
                    deck = ComputerAI.TempCardUse(_gameContainer);
                    tempList = new() { deck };
                    await _showActionProcesses.ShowChosenForEverybodyGetsOneAsync(tempList, selectedIndex);
                    await _everybodyOneProcesses.EverybodyGetsOneAsync(tempList, selectedIndex);
                    break;
                case EnumActionMain.LetsDoThatAgain:
                    selectedIndex = ComputerAI.CardToDoAgain(_actionContainer);
                    await _showActionProcesses.ShowLetsDoAgainAsync(selectedIndex);
                    var thisCard = _actionContainer.GetCardToDoAgain(selectedIndex);
                    await _actionContainer.DoAgainProcessPart1Async(selectedIndex);
                    await _playProcesses.PlayCardAsync(thisCard);
                    break;
                case EnumActionMain.LetsSimplify:
                    tempList = ComputerAI.SimplifyRules(_gameContainer, _actionContainer);
                    await _showActionProcesses.ShowRulesSimplifiedAsync(tempList);
                    await _finalRuleProcesses.SimplifyRulesAsync(tempList);
                    break;
                case EnumActionMain.RotateHands:
                    var thisDirection = ComputerAI.GetDirection();
                    await _showActionProcesses.ShowDirectionAsync((int)thisDirection);
                    await _rotateTradeHandProcesses.RotateHandAsync(thisDirection);
                    break;
                case EnumActionMain.TradeHands:
                    selectedIndex = ComputerAI.GetPlayerSelectedIndex(_actionContainer);
                    await _showActionProcesses.ShowTradeHandAsync(selectedIndex);
                    await _rotateTradeHandProcesses.TradeHandAsync(selectedIndex);
                    break;
                case EnumActionMain.TrashANewRule:
                    selectedIndex = ComputerAI.RuleToTrash(_actionContainer);
                    await _showActionProcesses.ShowRuleTrashedAsync(selectedIndex);
                    await _finalRuleProcesses.TrashNewRuleAsync(selectedIndex);
                    break;
                case EnumActionMain.UseWhatYouTake:
                    if (SaveRoot!.SavedActionData.SelectedIndex == -1)
                    {
                        selectedIndex = ComputerAI.GetPlayerSelectedIndex(_actionContainer);
                        await _showActionProcesses.ShowPlayerForCardChosenAsync(selectedIndex);
                        await _actionContainer.ChosePlayerOnActionAsync(selectedIndex);
                        await ContinueTurnAsync();
                        return;
                    }
                    deck = ComputerAI.UseTake(_gameContainer, _actionContainer, SaveRoot.SavedActionData.SelectedIndex);
                    int index = _actionContainer.GetPlayerIndex(SaveRoot.SavedActionData.SelectedIndex);
                    await _playProcesses.PlayUseTakeAsync(deck, index);
                    break;
                default:
                    throw new CustomBasicException("No action found for computer on action screen");
            }
            return;
        }
        var thisStatus = StatusEndRegularTurn();
        int numberNeeded;
        switch (thisStatus)
        {
            case EnumEndTurnStatus.Successful:
                if (BasicData!.MultiPlayer)
                {
                    await Network!.SendEndTurnAsync();
                }
                await EndTurnAsync();
                break;
            case EnumEndTurnStatus.Hand:
                numberNeeded = SingleInfo!.MainHandList.Count - SaveRoot!.HandLimit;
                if (numberNeeded <= 0)
                {
                    throw new CustomBasicException("No hand limit or already followed it");
                }
                await _discardProcesses.DiscardFromHandAsync(ComputerAI.CardsToDiscardFromHand(_gameContainer, numberNeeded));
                break;
            case EnumEndTurnStatus.Play:
                var tempCard = ComputerAI.CardToPlay(_gameContainer);
                await _playProcesses.SendPlayAsync(tempCard.Deck);
                await _playProcesses.PlayCardAsync(tempCard);
                break;
            case EnumEndTurnStatus.Keeper:
                numberNeeded = SingleInfo!.KeeperList.Count - SaveRoot!.KeeperLimit;
                if (numberNeeded <= 0)
                {
                    throw new CustomBasicException("No keeper limit or already followed");
                }
                await _discardProcesses.DiscardKeepersAsync(ComputerAI.DiscardKeepers(_gameContainer, numberNeeded));
                break;
            case EnumEndTurnStatus.Goal:
                await _discardProcesses.DiscardGoalAsync(ComputerAI.GoalToRemove(_gameContainer));
                await _analyzeProcesses.AnalyzeQueAsync(); //i think.
                break;
            default:
                throw new CustomBasicException("Wrong");
        }
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        if (_model.Deck1!.CardExists(1))
        {
            throw new CustomBasicException("The first card cannot be in the deck");
        }
        LoadControls();
        SaveRoot!.LoadMod(_model);
        SaveRoot.ImmediatelyStartTurn = true;
        return base.StartSetUpAsync(isBeginning);
    }
    private void LastStepAll()
    {
        _actionContainer.SetUpGoals();
        if (SaveRoot!.CurrentAction > 0)
        {
            _gameContainer!.CurrentAction = (ActionCard)_gameContainer.DeckList!.GetSpecificItem(SaveRoot.CurrentAction);
        }
        else
        {
            _gameContainer!.CurrentAction = null;
        }
        _gameContainer.QuePlayList.Clear();
        _gameContainer.QuePlayList = SaveRoot.QueList.GetNewObjectListFromDeckList(_gameContainer.DeckList!).ToRegularDeckDict();
        SingleInfo = PlayerList!.GetSelf();
        _model.Keeper1!.HandList = SingleInfo.KeeperList;
        _actionContainer.SetUpFrames();
        _model.Goal1!.HandList = SaveRoot.GoalList;
        _keeperContainer.LoadSavedGame();
    }
    protected override async Task AddCardAsync(FluxxCardInformation thisCard, FluxxPlayerItem tempPlayer)
    {
        if (thisCard.Deck == 1)
        {
            throw new CustomBasicException("The basic rule can never be drawn");
        }
        if (_gameContainer.AllNewCards)
        {
            thisCard.Drew = false;
        }
        if (_gameContainer.DoDrawTemporary == false)
        {
            await base.AddCardAsync(thisCard, tempPlayer);
            return;
        }
        thisCard.Drew = false;
        _gameContainer!.TempActionHandList.Add(thisCard.Deck);
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "taxation":
                var thisList1 = await content.GetObjectsFromDataAsync(SingleInfo!.MainHandList); //i think.
                await _giveTaxationProcesses.GiveCardsForTaxationAsync(thisList1);
                return;
            case "emptytrash":
                var thisList2 = await content.GetNewObjectListFromDeckListAsync(_gameContainer.DeckList!);
                await _emptyTrashProcesses.FinishEmptyTrashAsync(thisList2);
                return;
            case "discardfromhand":
                var thisList3 = await content.GetSavedIntegerListAsync();
                await _discardProcesses.DiscardFromHandAsync(thisList3);
                return;
            case "discardkeepers":
                var thisList4 = await content.GetSavedIntegerListAsync();
                await _discardProcesses.DiscardKeepersAsync(thisList4);
                return;
            case "discardgoal":
                await _discardProcesses.DiscardGoalAsync(int.Parse(content));
                await _analyzeProcesses.AnalyzeQueAsync();
                return;
            case "trashkeeper":
            case "stealkeeper":
                KeeperPlayer thisKeep = await js1.DeserializeObjectAsync<KeeperPlayer>(content);
                bool isTrashed = status == "trashkeeper";
                BasicList<KeeperPlayer> tempList1 = new() { thisKeep };
                _keeperContainer.ShowSelectedKeepers(tempList1);
                if (Test!.NoAnimations == false)
                {
                    await Delay!.DelaySeconds(1);
                }
                await _finalKeeperProcesses.ProcessTrashStealKeeperAsync(thisKeep, isTrashed);
                return;
            case "exchangekeepers":
                BasicList<KeeperPlayer> thisList5 = await js1.DeserializeObjectAsync<BasicList<KeeperPlayer>>(content);
                _keeperContainer.ShowSelectedKeepers(thisList5);
                if (Test!.NoAnimations == false)
                {
                    await Delay!.DelaySeconds(1);
                }
                await _finalKeeperProcesses.ProcessExchangeKeepersAsync(thisList5.First(), thisList5.Last());
                return;
            case "firstrandom":
                await _playProcesses.PlayRandomCardAsync(int.Parse(content));
                return;
            case "usetake":
                var thisList6 = await content.GetSavedIntegerListAsync();
                if (_delegates.LoadMainScreenAsync == null)
                {
                    throw new CustomBasicException("Nobody is loading the main screen.  Rethink");
                }
                await _delegates.LoadMainScreenAsync.Invoke();
                await _playProcesses.PlayRandomCardAsync(thisList6.First(), thisList6.Last());
                return;
            case "simplifyrules":
                var thisList7 = await content.GetSavedIntegerListAsync();
                await _showActionProcesses.ShowRulesSimplifiedAsync(thisList7);
                await _finalRuleProcesses.SimplifyRulesAsync(thisList7);
                return;
            case "trashnewrule":
                await _showActionProcesses.ShowRuleTrashedAsync(int.Parse(content));
                await _finalRuleProcesses.TrashNewRuleAsync(int.Parse(content));
                return;
            case "doagain":
                await _showActionProcesses.ShowLetsDoAgainAsync(int.Parse(content));
                var thisCard = _actionContainer.GetCardToDoAgain(int.Parse(content));
                await _playProcesses.PlayCardAsync(thisCard);
                return;
            case "rotatehands":
                EnumDirection direction = await js1.DeserializeObjectAsync<EnumDirection>(content);
                await _showActionProcesses.ShowDirectionAsync((int) direction);
                await _rotateTradeHandProcesses.RotateHandAsync(direction);
                return;
            case "tradehands":
                await _showActionProcesses.ShowTradeHandAsync(int.Parse(content));
                await _rotateTradeHandProcesses.TradeHandAsync(int.Parse(content));
                return;
            case "choseplayerforcardchosen":
                await _showActionProcesses.ShowPlayerForCardChosenAsync(int.Parse(content));
                await ContinueTurnAsync();
                return;
            case "everybodygetsone":
                var thisList8 = await content.GetSavedIntegerListAsync();
                int selectedIndex = thisList8.Last();
                thisList8.RemoveLastItem();
                await _showActionProcesses.ShowChosenForEverybodyGetsOneAsync(thisList8, selectedIndex);
                await _everybodyOneProcesses.EverybodyGetsOneAsync(thisList8, selectedIndex);
                return;
            case "drawuse":
                await _showActionProcesses.ShowCardUseAsync(int.Parse(content));
                await _drawUseProcesses.DrawUsedAsync(int.Parse(content));
                return;
            case "playcard":
                SingleInfo = PlayerList!.GetWhoPlayer();
                await _playProcesses.PlayCardAsync(int.Parse(content));
                return;
            case "scramblekeepers":
                var thisList = PlayerList.Where(items => items.KeeperList.Count > 0).ToBasicList();
                if (thisList.Count < 2)
                {
                    throw new CustomBasicException("Must have at least 2 players with keepers in order to scramble keepers");
                }
                BasicList<BasicList<int>> finList = await js1.DeserializeObjectAsync<BasicList<BasicList<int>>>(content);
                if (finList.Count != thisList.Count)
                {
                    throw new CustomBasicException("When other player is scrambling keepers, does not reconcile");
                }
                int x = 0;
                thisList.ForEach(thisPlayer =>
                {
                    if (thisPlayer.KeeperList.Count != finList[x].Count)
                    {
                        throw new CustomBasicException("Wrong count on other player end");
                    }
                    thisPlayer.KeeperList.Clear();
                    foreach (var thisItem in finList[x])
                    {
                        thisPlayer.KeeperList.Add((KeeperCard)_gameContainer.DeckList!.GetSpecificItem(thisItem));
                    }
                    x++;
                });
                await _analyzeProcesses.AnalyzeQueAsync();
                return;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override async Task StartNewTurnAsync()
    {
        await base.StartNewTurnAsync();
        SaveRoot!.DoAnalyze = false;
        SingleInfo = PlayerList!.GetWhoPlayer();
        if (_gameContainer!.CurrentAction != null)
        {
            throw new CustomBasicException("Current action must be nothing to begin with");
        }
        if (_gameContainer.QuePlayList.Count > 0)
        {
            throw new CustomBasicException("Cannot have any items on the que list when starting a new turn");
        }
        SaveRoot.CardsPlayed = 0;
        SaveRoot.CardsDrawn = 0;
        SaveRoot.AnotherTurn = false;
        _model!.OtherTurn = "";
        SaveRoot.PlaysLeft = 0;
        SaveRoot.PlayBonus = 0;
        int extras = _gameContainer.IncreaseAmount();
        if (SaveRoot.RuleList.Any(items => items.Deck == EnumRuleText.NoHandBonus) && SingleInfo.MainHandList.Count == 0)
        {
            SaveRoot.PreviousBonus += 3;
        }
        else
        {
            SaveRoot.PreviousBonus = 0;
        }
        await ContinueTurnAsync();
    }
    public override async Task ContinueTurnAsync()
    {
        if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Computer && Test!.ComputerEndsTurn)
        {
            await EndTurnAsync(); //just end turn and don't bother even drawing more cards.
            return;
        }
        PlayerList.ForEach(player => player.UpdateKeepers());
        if (SaveRoot!.DoAnalyze)
        {
            await _analyzeProcesses.AnalyzeQueAsync();
            return;
        }
        int wins = _gameContainer.WhoWonGame();
        if (wins > 0)
        {
            SingleInfo = PlayerList![wins];
            await ShowWinAsync();
            return;
        }
        SingleInfo = PlayerList!.GetWhoPlayer();
        _analyzeProcesses.AnalyzeRules();
        if (LeftToDraw > 0)
        {
            await DrawAsync();
            return;
        }
        var tempList = PlayerList.ToBasicList();
        tempList.RemoveSpecificItem(SingleInfo);
        var thisPlayer = tempList.Where(items => items.ObeyedRulesWhenNotYourTurn() == false).Take(1).SingleOrDefault();
        if (thisPlayer != null)
        {
            OtherTurn = thisPlayer.Id;
        }
        if (_gameContainer!.CurrentAction != null && _gameContainer.CurrentAction.Deck == EnumActionMain.Taxation)
        {
            if (_gameContainer.LoadGiveAsync != null)
            {
                await _gameContainer.LoadGiveAsync();
            }
        }
        else if (thisPlayer == null)
        {
            OtherTurn = 0;
        }
        if (OtherTurn > 0)
        {
            SingleInfo = PlayerList.GetOtherPlayer();
            _model!.OtherTurn = SingleInfo.NickName;
        }
        else
        {
            _model!.OtherTurn = "";
        }
        if (_gameContainer.IsFirstPlayRandom())
        {
            OtherTurn = await PlayerList.CalculateWhoTurnAsync();
            SingleInfo = PlayerList.GetOtherPlayer();
            _model.OtherTurn = SingleInfo.NickName;
        }
        await base.ContinueTurnAsync();
    }
    public override async Task EndTurnAsync()
    {
        _command.ManuelFinish = true; //because it could be somebody else's turn.
        SingleInfo = PlayerList!.GetWhoPlayer();
        SingleInfo.MainHandList.UnhighlightObjects();
        _model.Goal1!.EndTurn();
        _model.Keeper1!.EndTurn();
        if (_gameContainer.QuePlayList.Count > 0)
        {
            await _playProcesses.PlayCardAsync(_gameContainer.QuePlayList.First());
            return;
        }
        if (SaveRoot!.AnotherTurn == false)
        {
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
        }
        await StartNewTurnAsync();
    }
    public EnumEndTurnStatus StatusEndRegularTurn()
    {
        if (_gameContainer.NeedsToRemoveGoal())
        {
            return EnumEndTurnStatus.Goal;
        }
        if (SaveRoot!.CardsPlayed < SaveRoot.PlayLimit && SaveRoot.PlayLimit > -1 && SingleInfo!.MainHandList.Count > 0)
        {
            return EnumEndTurnStatus.Play;
        }
        if (SaveRoot.PlayLimit == -1 && SingleInfo!.MainHandList.Count > 0)
        {
            return EnumEndTurnStatus.Play; //because you have to play all.
        }
        if (SaveRoot.KeeperLimit > -1)
        {
            if (SingleInfo!.KeeperList.Count > SaveRoot.KeeperLimit)
            {
                return EnumEndTurnStatus.Keeper;
            }
        }
        if (SaveRoot.HandLimit > -1)
        {
            if (SingleInfo!.MainHandList.Count > SaveRoot.HandLimit)
            {
                return EnumEndTurnStatus.Hand;
            }
        }
        return EnumEndTurnStatus.Successful; //this means you obeyed all the rules and can end your turn.
    }
    protected override async Task AfterDrawingAsync()
    {
        _gameContainer.DoDrawTemporary = false;
        _gameContainer.AllNewCards = false;
        if (SaveRoot!.DoAnalyze)
        {
            await _analyzeProcesses.AnalyzeQueAsync();
            return;
        }
        if (_gameContainer!.CurrentAction == null)
        {
            await ContinueTurnAsync();
            return;
        }
        await _analyzeProcesses.AnalyzeQueAsync();
    }
    protected override async Task LoadPossibleOtherScreensAsync()
    {

        EnumActionScreen thisScreen = _gameContainer.ScreenToLoad(OtherTurn);
        if (thisScreen == EnumActionScreen.ActionScreen)
        {
            await _actionContainer.PrepActionScreenAsync(_gameContainer, _delegates, _loadActionProcesses);
            return;
        }
        if (thisScreen == EnumActionScreen.KeeperScreen)
        {

            await _keeperContainer.LoadKeeperScreenAsync();
            return;
        }
    }
    public override Task PopulateSaveRootAsync()
    {
        SaveRoot!.QueList = _gameContainer!.QuePlayList.GetDeckListFromObjectList();
        if (_gameContainer.CurrentAction == null)
        {
            SaveRoot.CurrentAction = 0;
            SaveRoot.SavedActionData.SelectedIndex = -1;
        }
        else
        {
            SaveRoot.CurrentAction = (int)_gameContainer.CurrentAction.Deck;
            if (_gameContainer.CurrentAction.Deck == EnumActionMain.UseWhatYouTake)
            {
                SaveRoot.SavedActionData.SelectedIndex = _actionContainer.IndexPlayer;
            }
            else
            {
                SaveRoot.SavedActionData.SelectedIndex = -1;
            }
        }
        return base.PopulateSaveRootAsync();
    }
}