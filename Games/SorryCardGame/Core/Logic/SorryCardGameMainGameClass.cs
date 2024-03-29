namespace SorryCardGame.Core.Logic;
[SingletonGame]
public class SorryCardGameMainGameClass
    : CardGameClass<SorryCardGameCardInformation, SorryCardGamePlayerItem, SorryCardGameSaveInfo>
    , IBeginningComboCardsColors<EnumColorChoices, SorryCardGameCardInformation, SorryCardGamePlayerItem, SorryCardGameSaveInfo>
    , IMiscDataNM, IAfterColorProcesses, ISerializable
{
    private readonly SorryCardGameVMData _model;
    private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
    private readonly SorryCardGameGameContainer _gameContainer; //if we don't need it, take it out.
    private readonly IToast _toast;
    public SorryCardGameMainGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        SorryCardGameVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<SorryCardGameCardInformation> cardInfo,
        CommandContainer command,
        SorryCardGameGameContainer gameContainer,
        ISystemError error,
        IToast toast
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, error, toast)
    {
        _model = currentMod;
        _command = command;
        _gameContainer = gameContainer;
        _toast = toast;
    }
    public override bool CanMakeMainOptionsVisibleAtBeginning => PlayerList.DidChooseColors();
    private bool _wasNew;
    private bool _didPlay;
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
    public void EraseColors()
    {
        PlayerList.EraseColors();
    }
    public override Task ShowWinAsync()
    {
        EraseColors();
        return base.ShowWinAsync();
    }
    public override Task FinishGetSavedAsync()
    {
        LoadControls();
        SaveRoot!.LoadMod(_model!);
        if (SaveRoot.OtherPileData != null)
        {
            _model!.Pile1!.SavedDiscardPiles(SaveRoot!.OtherPileData);
        }
        return base.FinishGetSavedAsync();
    }
    private void LoadControls() //has to do no matter what now.
    {
        PlayerList!.ForEach(thisPlayer => thisPlayer.Load(this, _command));
    }
    protected override async Task LoadPossibleOtherScreensAsync()
    {
        if (PlayerList.DidChooseColors())
        {
            return;
        }
        if (SingleInfo == null)
        {
            throw new CustomBasicException("Single info cannot be null when trying to load other possible screens.  Rethink");
        }
        if (MiscDelegates.ContinueColorsAsync == null)
        {
            return;
        }
        await MiscDelegates.ContinueColorsAsync.Invoke();
    }
    public override async Task ContinueTurnAsync()
    {
        if (PlayerList.DidChooseColors() == false)
        {
            PrepStartTurn();
            await base.ContinueTurnAsync();
            return;
        }
        if (SaveRoot!.GameStatus == EnumGameStatus.ChoosePlayerToSorry)
        {
            SaveRoot.Instructions = "Choose a player to sorry";
        }
        else if (SaveRoot.GameStatus == EnumGameStatus.Regular)
        {
            SaveRoot.Instructions = "Please play a card";
        }
        else if (SaveRoot.GameStatus == EnumGameStatus.HasDontBeSorry)
        {
            SaveRoot.Instructions = "Waiting to see if player has don't be sorry";
        }
        else if (SaveRoot.GameStatus == EnumGameStatus.WaitForSorry21)
        {
            SaveRoot.Instructions = "Waiting to see if a player has a sorry at 21";
        }
        if (SaveRoot.GameStatus != EnumGameStatus.WaitForSorry21 && SaveRoot.GameStatus != EnumGameStatus.HasDontBeSorry)
        {
            _model.PlayerHand1!.AutoSelect = EnumHandAutoType.SelectAsMany;
            await base.ContinueTurnAsync();
            return;
        }
        if (SaveRoot.GameStatus == EnumGameStatus.HasDontBeSorry)
        {
            if (SaveRoot.LastSorry == EnumSorry.At21)
            {
                await PossibleStartTimerAsync(WhoTurn);
                return;
            }
            if (OtherTurn == 0)
            {
                throw new CustomBasicException("Can't have otherturn of 0 when the status is don't be sorry");
            }
            if (OtherTurn == WhoTurn)
            {
                throw new CustomBasicException("Can't have otherturn the same as the players turn for don't be sorry");
            }
            await PossibleStartTimerAsync(OtherTurn);
            return;
        }
        if (SaveRoot.GameStatus == EnumGameStatus.WaitForSorry21)
        {
            var tempPlayer = PlayerList!.GetSelf();
            if (tempPlayer.PlayerCategory != EnumPlayerCategory.Self)
            {
                throw new CustomBasicException("Not self");
            }
            if (tempPlayer.OtherTurn == false)
            {
                await ShowHumanCanPlayAsync();
                return;
            }
            if (tempPlayer.PlayerCategory == EnumPlayerCategory.Computer)
            {
                await ComputerTurnAsync();
                return;
            }
            Network!.IsEnabled = true;
            return;
        }
        throw new CustomBasicException("Don't know what to do now");
    }
    protected override async Task ComputerTurnAsync()
    {
        if (PlayerList.DidChooseColors() == false)
        {
            if (MiscDelegates.ComputerChooseColorsAsync == null)
            {
                throw new CustomBasicException("The computer choosing color was never handled.  Rethink");
            }
            await MiscDelegates.ComputerChooseColorsAsync.Invoke();
            return;
        }
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelaySeconds(.5);
        }
        if (SaveRoot!.GameStatus == EnumGameStatus.HasDontBeSorry)
        {
            throw new CustomBasicException("The computer should never have a don't be sorry");
        }
        await EndTurnAsync();
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        LoadControls();
        SaveRoot!.LoadMod(_model!);
        SaveRoot.UpTo = 0;
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.Color = EnumColorChoices.None;
            if (Test!.DoubleCheck == false)
            {
                thisPlayer.HowManyAtHome = 0;
            }
            else
            {
                thisPlayer.HowManyAtHome = 1;
            }
        });
        _model!.OtherPile!.ClearCards();
        SaveRoot.WasTie = false;
        return base.StartSetUpAsync(isBeginning);
    }
    protected override bool AutoOtherPileCurrentOnly()
    {
        return false;
    }
    private async Task PossibleStartTimerAsync(int player)
    {
        await SaveStateAsync();
        var tempPlayer = PlayerList![player];
        if (tempPlayer.PlayerCategory == EnumPlayerCategory.Computer)
        {
            throw new CustomBasicException("Computer should not be playing");
        }
        if (tempPlayer.PlayerCategory == EnumPlayerCategory.OtherHuman)
        {
            Network!.IsEnabled = true;
            return;
        }
        await ShowHumanCanPlayAsync();
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "sorrycard":
                var firstSend = await GetSentPlayAsync(content);
                if (SaveRoot!.GameStatus == EnumGameStatus.Regular || SaveRoot.GameStatus == EnumGameStatus.ChoosePlayerToSorry)
                {
                    EnableMessages();
                    return;
                }
                var tempPlayer1 = PlayerList![firstSend.Player];
                if (tempPlayer1.OtherTurn == true)
                {
                    EnableMessages();
                    return;
                }
                var thisCard = tempPlayer1.MainHandList.GetSpecificItem(firstSend.Deck);
                await PlaySorryCardAsync(thisCard, firstSend.Player);
                return;
            case "sorryplayer":
                await SorryPlayerAsync(int.Parse(content));
                return;
            case "regularplay":
                BasicList<int> thisList = await js1.DeserializeObjectAsync<BasicList<int>>(content);
                var newCol = thisList.GetNewObjectListFromDeckList(_gameContainer.DeckList!);
                await PlaySeveralCards(newCol);
                return;
            case "timeout":
                if (SaveRoot!.GameStatus == EnumGameStatus.Regular || SaveRoot.GameStatus == EnumGameStatus.ChoosePlayerToSorry)
                {
                    EnableMessages();
                    return;
                }
                var finPlayer = PlayerList![int.Parse(content)];
                if (finPlayer.OtherTurn == true)
                {
                    EnableMessages();
                    return;
                }
                await NoSorryAsync(finPlayer.Id);
                return;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override async Task StartNewTurnAsync()
    {
        await base.StartNewTurnAsync();
        if (PlayerList.DidChooseColors() == false)
        {
            await ContinueTurnAsync();
            return;
        }
        PlayerList!.ForEach(thisPlayer => thisPlayer.OtherTurn = true); //has to manually set to false.
        SaveRoot!.GameStatus = EnumGameStatus.Regular;
        SaveRoot.LastSorry = EnumSorry.None;
        OtherTurn = 0;
        await base.StartNewTurnAsync();
        SingleInfo = PlayerList.GetWhoPlayer();
        await StartDrawingAsync();
    }
    public override async Task EndTurnAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        SingleInfo.MainHandList.UnhighlightObjects();
        if (SaveRoot!.UpTo > 21)
        {
            await BustAsync();
        }
        else if (SaveRoot.UpTo == 21 && SaveRoot.GameStatus == EnumGameStatus.Regular)
        {
            SingleInfo.HowManyAtHome++;
            if (BasicData!.MultiPlayer == true)
            {
                SaveRoot.GameStatus = EnumGameStatus.WaitForSorry21;
                PlayerList.ForConditionalItems(items => items.Id != SingleInfo.Id, thisPlayer =>
                {
                    thisPlayer.OtherTurn = false;
                });
                SingleInfo.OtherTurn = true;
                _command.ManuelFinish = true;
                await ContinueTurnAsync();
                return;
            }
            else
            {
                await ClearPointsAsync(); //because the computer will never decide whether it has sorry21s.
            }
        }
        else if (SaveRoot.UpTo == 21)
        {
            throw new CustomBasicException("Don't know what to do.  Could be a never ending loop.  Should have cleared the points first");
        }
        if (PlayerList.Any(items => items.HowManyAtHome == 4))
        {
            try
            {
                SingleInfo = PlayerList.Single(items => items.HowManyAtHome == 4);
            }
            catch
            {
                throw new CustomBasicException("Possible tie.  Rethink");
            }
            SaveRoot.GameStatus = EnumGameStatus.Regular;
            await ShowWinAsync();
            return;
        }
        _command.ManuelFinish = true; //could be somebody else's turn.
        WhoTurn = await PlayerList.CalculateWhoTurnAsync();
        await StartNewTurnAsync();
    }
    private async Task BustAsync()
    {
        _toast.ShowInfoToast($"{SingleInfo!.NickName} has busted.  Everybody else gets one home");
        PlayerList!.ForConditionalItems(items => items.Id != SingleInfo.Id, thisPlayer =>
        {
            thisPlayer.HowManyAtHome++;
        });
        SaveRoot!.UpTo = 0;
        await ClearPointsAsync();
    }
    public async Task ClearPointsAsync()
    {
        if (_model!.OtherPile!.PileEmpty())
        {
            return;
        }
        var newList = _model!.OtherPile.DiscardList().ToRegularDeckDict();
        newList.Add(_model!.OtherPile.GetCardInfo());
        newList.Reverse();
        await newList.ForEachAsync(async thisCard =>
        {
            _model!.OtherPile.RemoveCardFromPile(thisCard);
            await AnimatePlayAsync(thisCard);
            if (Test!.NoAnimations == false)
            {
                await Delay!.DelaySeconds(.25);
            }
        });
        _model!.OtherPile.ClearCards();
        SaveRoot!.UpTo = 0;
    }
    private async Task StartDrawingAsync()
    {
        if (SingleInfo!.MainHandList.Count >= 5)
        {
            await ContinueTurnAsync();
            return; //because you already have at least 5 cards.
        }
        PlayerDraws = WhoTurn;
        _wasNew = SingleInfo.MainHandList.Count == 0;
        LeftToDraw = 5 - SingleInfo.MainHandList.Count;
        if (LeftToDraw == 1)
        {
            LeftToDraw = 0;
            PlayerDraws = 0;
        }
        await DrawAsync();
    }
    protected override bool CardToCurrentPile()
    {
        return false;
    }
    private async Task Draw2CardsAsync()
    {
        _didPlay = true;
        LeftToDraw = 2;
        _wasNew = false;
        await DrawAsync();
    }
    protected override async Task AfterDrawingAsync()
    {
        if (_wasNew == true)
        {
            SingleInfo!.MainHandList.UnhighlightObjects();
        }
        if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
        {
            SortCards();
        }
        if (_didPlay == false)
        {
            await base.AfterDrawingAsync();
            return;
        }
        _didPlay = false;
        await EndTurnAsync();
    }
    private void EnableMessages()
    {
        int thisPlayer = PlayerList!.GetSelf().Id;
        if (thisPlayer == WhoTurn)
        {
            return; //because your turn.
        }
        Network!.IsEnabled = true;
    }
    private static async Task<SorryPlay> GetSentPlayAsync(string message)
    {
        return await js1.DeserializeObjectAsync<SorryPlay>(message);
    }
    async Task IAfterColorProcesses.AfterChoosingColorsAsync()
    {
        WhoTurn = WhoStarts;
        _command.UpdateAll();
        await StartNewTurnAsync();
    }
    public async Task SorryPlayerAsync(int player)
    {
        if (SaveRoot!.GameStatus != EnumGameStatus.ChoosePlayerToSorry)
        {
            throw new CustomBasicException("Can't sorry a player since its choosing who to sorry");
        }
        OtherTurn = player;
        var tempPlayer = PlayerList![OtherTurn];
        if (tempPlayer.HowManyAtHome == 0)
        {
            throw new CustomBasicException("Had none at home. Therefore; can't sorry a player");
        }
        tempPlayer.HowManyAtHome--;
        SaveRoot.LastSorry = EnumSorry.Regular;
        _command.ManuelFinish = true;
        if (BasicData!.MultiPlayer == false)
        {
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Computer)
            {
                throw new CustomBasicException("The computer can't sorry a player");
            }
            await EndTurnAsync();
            return;
        }
        SaveRoot.GameStatus = EnumGameStatus.HasDontBeSorry;
        PlayerList.ForEach(thisPlayer => thisPlayer.OtherTurn = true);
        tempPlayer.OtherTurn = false;
        await ContinueTurnAsync();
    }
    public async Task NoSorryAsync(int player)
    {
        if (SaveRoot!.GameStatus == EnumGameStatus.HasDontBeSorry)
        {
            await ClearPointsAsync();
            await EndTurnAsync();
            return;
        }
        if (SaveRoot.GameStatus != EnumGameStatus.WaitForSorry21)
        {
            throw new CustomBasicException("Don't know of any other cases where it needs a NoSorry routine");
        }
        var tempPlayer = PlayerList![player];
        tempPlayer.OtherTurn = true;
        if (!PlayerList.Any(items => items.OtherTurn == false))
        {
            await ClearPointsAsync();
            await EndTurnAsync();
            return;
        }
        await ContinueTurnAsync();
    }
    private async Task PlaySingleCardAsync(SorryCardGameCardInformation thisCard)
    {
        thisCard.Drew = false;
        thisCard.IsSelected = false;
        if (thisCard.Category == EnumCategory.Regular)
        {
            SaveRoot!.UpTo += thisCard.Value;
        }
        else if (thisCard.Category == EnumCategory.Slide)
        {
            SaveRoot!.UpTo = thisCard.Value;
        }
        else if (thisCard.Category == EnumCategory.Switch)
        {
            PlayerList!.ChangeReverse();
        }
        if (SingleInfo!.MainHandList.ObjectExist(thisCard.Deck))
        {
            SingleInfo.MainHandList.RemoveObjectByDeck(thisCard.Deck);
        }
        else if (OtherTurn > 0)
        {
            var tempPlayer = PlayerList!.GetOtherPlayer();
            if (tempPlayer.MainHandList.ObjectExist(thisCard.Deck))
            {
                tempPlayer.MainHandList.RemoveObjectByDeck(thisCard.Deck);
            }
        }
        _command.UpdateAll();
        await Aggregator.AnimateCardAsync(thisCard, EnumAnimcationDirection.StartDownToCard, "otherpile", finalAction: () => _model!.OtherPile!.AddCard(thisCard));
    }
    public async Task PlaySorryCardAsync(SorryCardGameCardInformation thisCard, int player)
    {
        if (thisCard.Sorry != EnumSorry.At21 && thisCard.Sorry != EnumSorry.Dont)
        {
            throw new CustomBasicException("Can't play a sorry card except for a sorry at 21 or don't be sorry");
        }
        if (thisCard.Sorry == EnumSorry.Dont)
        {
            if (SaveRoot!.GameStatus != EnumGameStatus.HasDontBeSorry)
            {
                throw new CustomBasicException("Don't be sorry can't be played unless the status is don't be sorry");
            }
        }
        if (thisCard.Sorry == EnumSorry.At21)
        {
            if (SaveRoot!.GameStatus != EnumGameStatus.WaitForSorry21)
            {
                throw new CustomBasicException("Can't play a sorry at 21 because nobody is at 21");
            }
        }
        SorryCardGamePlayerItem tempPlayer;
        _command.ManuelFinish = true; //because you can't go automatically now.
        if (thisCard.Sorry == EnumSorry.At21)
        {
            if (BasicData!.MultiPlayer == false)
            {
                throw new CustomBasicException("Can't be at 21 for single player games currently");
            }
            SaveRoot!.GameStatus = EnumGameStatus.HasDontBeSorry;
            OtherTurn = player;
            SaveRoot.LastSorry = EnumSorry.At21;
            SingleInfo!.HowManyAtHome--;
            tempPlayer = PlayerList!.GetOtherPlayer();
            tempPlayer.HowManyAtHome++;
            await PlaySingleCardAsync(thisCard);
            PlayerList.ForEach(thisPlayer => thisPlayer.OtherTurn = true);
            SingleInfo.OtherTurn = false;
            await ContinueTurnAsync();
            return;
        }
        if (thisCard.Sorry == EnumSorry.Dont)
        {
            if (SaveRoot!.LastSorry == EnumSorry.Regular)
            {
                if (player != OtherTurn)
                {
                    throw new CustomBasicException("Player must be the player who was the sorry was played on");
                }
                tempPlayer = PlayerList!.GetOtherPlayer();
                tempPlayer.HowManyAtHome++;
                SingleInfo = PlayerList.GetWhoPlayer();
                await PlaySingleCardAsync(thisCard);
                await EndTurnAsync();
                return;
            }
            if (player != WhoTurn)
            {
                throw new CustomBasicException("The current player whose turn it is must be the one who plays the don't be sorry");
            }
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.HowManyAtHome++;
            tempPlayer = PlayerList.GetOtherPlayer();
            tempPlayer.HowManyAtHome--;
            await PlaySingleCardAsync(thisCard);
            await ClearPointsAsync();
            await EndTurnAsync();
            return;
        }
        throw new CustomBasicException("Don't know what to do from here");
    }
    public async Task PlaySeveralCards(IDeckDict<SorryCardGameCardInformation> thisList)
    {
        if (SaveRoot!.GameStatus == EnumGameStatus.ChoosePlayerToSorry)
        {
            throw new CustomBasicException("Can't choose player to sorry and play the cards");
        }
        _command.ManuelFinish = true;
        if (thisList.Count == 1)
        {
            var tempCard = thisList.First();
            await PlaySingleCardAsync(tempCard);
            if (Test!.ImmediatelyEndGame)
            {
                await ShowWinAsync();
                return;
            }
            if (tempCard.Sorry == EnumSorry.Regular)
            {
                if (PlayerList.Any(items => items.Id != WhoTurn && items.HowManyAtHome > 0))
                {
                    SaveRoot.GameStatus = EnumGameStatus.ChoosePlayerToSorry;
                    await ContinueTurnAsync();
                    return;
                }
            }
            if (SaveRoot.GameStatus == EnumGameStatus.Regular)
            {
                if (tempCard.Category == EnumCategory.Take2)
                {
                    if (SingleInfo!.MainHandList.Count <= 5)
                    {
                        await Draw2CardsAsync();
                        return;
                    }
                }
                await EndTurnAsync();
                return;
            }
            if (SaveRoot.GameStatus == EnumGameStatus.HasDontBeSorry)
            {
                throw new CustomBasicException("Must run a different process for don't be sorry");
            }
            if (SaveRoot.GameStatus == EnumGameStatus.WaitForSorry21)
            {
                throw new CustomBasicException("Must run a different process for wait for sorry 21");
            }
            throw new CustomBasicException("Wrong");
        }
        thisList = thisList.OrderByDescending(items => items.Category).ThenBy(items => items.Value).ToRegularDeckDict();
        await thisList.ForEachAsync(async thisCard =>
        {
            await PlaySingleCardAsync(thisCard);
            if (Test!.NoAnimations == false)
            {
                await Delay!.DelaySeconds(.3);
            }
        });
        await EndTurnAsync();
    }
    private bool IsSingleValid(SorryCardGameCardInformation thisCard)
    {
        if (thisCard.Category == EnumCategory.Sorry)
        {
            if (thisCard.Sorry == EnumSorry.Regular)
            {
                return PlayerList.Any(items => items.Id != WhoTurn && items.HowManyAtHome > 0);
            }
            if (thisCard.Sorry == EnumSorry.At21 || thisCard.Sorry == EnumSorry.Dont)
            {
                return false;
            }
        }
        if (thisCard.Category == EnumCategory.Take2)
        {
            return SingleInfo!.MainHandList.Count < 7;
        }
        return thisCard.Category != EnumCategory.Play2;
    }
    public bool IsValidMove(IDeckDict<SorryCardGameCardInformation> thisList)
    {
        if (thisList.Count > 1)
        {
            if (thisList.Count != 3)
            {
                return false;
            }
            if (thisList.Count(items => items.Category == EnumCategory.Play2) != 1)
            {
                return false;
            }
            if (thisList.Count(items => items.Category == EnumCategory.Regular) != 2)
            {
                return false;
            }
            return true;
        }
        var thisCard = thisList.Single();
        bool rets = IsSingleValid(thisCard);
        if (rets == true)
        {
            return true;
        }
        if (!SingleInfo!.MainHandList.Any(items => IsSingleValid(items)))
        {
            return true;
        }
        return false;
    }
}