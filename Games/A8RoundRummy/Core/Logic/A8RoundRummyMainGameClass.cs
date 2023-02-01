namespace A8RoundRummy.Core.Logic;
[SingletonGame]
public class A8RoundRummyMainGameClass
    : CardGameClass<A8RoundRummyCardInformation, A8RoundRummyPlayerItem, A8RoundRummySaveInfo>
    , IMiscDataNM, ISerializable
{
    private readonly A8RoundRummyVMData _model;
    private readonly CommandContainer _command;
    private readonly A8RoundRummyGameContainer _gameContainer;
    private readonly IToast _toast;
    internal A8RoundRummyCardInformation? LastCard { get; set; }
    internal bool LastSuccessful { get; set; }
    public bool WasGuarantee { get; set; }
    public A8RoundRummyCardInformation? CardForDiscard { get; set; }
    public EnumPlayerStatus PlayerStatus { get; set; }
    private bool _wasNew;
    public A8RoundRummyMainGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        A8RoundRummyVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<A8RoundRummyCardInformation> cardInfo,
        CommandContainer command,
        A8RoundRummyGameContainer gameContainer,
        ISystemError error,
        IToast toast
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, error, toast)
    {
        _model = currentMod;
        _command = command;
        _gameContainer = gameContainer;
        _toast = toast;
    }
    public override Task FinishGetSavedAsync()
    {
        LoadControls();
        _wasNew = false;
        CardForDiscard = null;
        WasGuarantee = false;
        return base.FinishGetSavedAsync();
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
        await Task.CompletedTask;
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        LoadControls();
        PlayerList!.ForEach(thisPlayer => thisPlayer.TotalScore = 0);
        _wasNew = false;
        WasGuarantee = false;
        CardForDiscard = null;
        BasicList<RoundClass> tempList = new();
        RoundClass thisRound = new();
        thisRound.Description = "Round 1:  Same Color";
        thisRound.Category = EnumCategory.Colors;
        thisRound.Rummy = EnumRummyType.Regular;
        thisRound.Points = 1;
        tempList.Add(thisRound);
        thisRound = new();
        thisRound.Description = "Round 2:  Same Shape";
        thisRound.Category = EnumCategory.Shapes;
        thisRound.Rummy = EnumRummyType.Regular;
        thisRound.Points = 2;
        tempList.Add(thisRound);
        thisRound = new();
        thisRound.Description = "Round 3:  Same Shape And Color";
        thisRound.Category = EnumCategory.Both;
        thisRound.Rummy = EnumRummyType.Regular;
        thisRound.Points = 3;
        tempList.Add(thisRound);
        thisRound = new();
        thisRound.Description = "Round 4:  Straight";
        thisRound.Category = EnumCategory.None;
        thisRound.Rummy = EnumRummyType.Straight;
        thisRound.Points = 4;
        tempList.Add(thisRound);
        thisRound = new();
        thisRound.Description = "Round 5:  Straight Same Color";
        thisRound.Category = EnumCategory.Colors;
        thisRound.Rummy = EnumRummyType.Straight;
        thisRound.Points = 5;
        tempList.Add(thisRound);
        thisRound = new();
        thisRound.Description = "Round 6:  Straight Same Shape";
        thisRound.Category = EnumCategory.Shapes;
        thisRound.Rummy = EnumRummyType.Straight;
        thisRound.Points = 6;
        tempList.Add(thisRound);
        thisRound = new();
        thisRound.Description = "Round 7:  Straight Same Shape And Color";
        thisRound.Category = EnumCategory.Both;
        thisRound.Rummy = EnumRummyType.Straight;
        thisRound.Points = 7;
        tempList.Add(thisRound);
        thisRound = new();
        thisRound.Description = "Round 8:  Same Number";
        thisRound.Category = EnumCategory.None;
        thisRound.Rummy = EnumRummyType.Kinds;
        thisRound.Points = 8;
        tempList.Add(thisRound);
        if (Test!.DoubleCheck)
        {
            tempList.KeepConditionalItems(items => items.Points >= 4);
        }
        SaveRoot!.RoundList.ReplaceRange(tempList);
        return base.StartSetUpAsync(isBeginning);
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "goout":
                MultiplayerOut thisOut = await js1.DeserializeObjectAsync<MultiplayerOut>(content);
                WasGuarantee = thisOut.WasGuaranteed;
                CardForDiscard = _gameContainer.DeckList!.GetSpecificItem(thisOut.Deck);
                await GoOutAsync();
                break;
            case "reversecards":
                var list = await PopulateCardsFromTurnHandAsync(content);
                await DiscardAllCardsAsync(list);
                break;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override async Task StartNewTurnAsync()
    {
        await base.StartNewTurnAsync();
        await ShowNextTurnAsync(); //i think here too.
        PlayerDraws = 0;
        if (PlayerStatus != EnumPlayerStatus.Regular)
        {
            PlayerStatus = EnumPlayerStatus.Regular;
            LeftToDraw = 0;
            await DrawAsync();
            return;
        }
        await ContinueTurnAsync();
    }
    public override async Task EndTurnAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        SingleInfo.MainHandList.UnhighlightObjects();
        _wasNew = false;
        _command.ManuelFinish = true; //because it could be somebody else's turn.
        WhoTurn = await PlayerList.CalculateWhoTurnAsync(true);
        await StartNewTurnAsync();
    }
    public override async Task ContinueTurnAsync()
    {
        await ShowNextTurnAsync();
        await base.ContinueTurnAsync();
    }
    private async Task ShowNextTurnAsync()
    {
        A8RoundRummyPlayerItem newPlayer;
        int newTurn = await PlayerList!.CalculateWhoTurnAsync();
        newPlayer = PlayerList[newTurn];
        _model!.NextTurn = newPlayer.NickName;
    }
    protected override bool ShowNewCardDrawn(A8RoundRummyPlayerItem tempPlayer)
    {
        if (_wasNew == true)
        {
            return false;
        }
        return base.ShowNewCardDrawn(tempPlayer);
    }
    public bool CanProcessDiscard(out bool pickUp, out int deck, out string message)
    {
        message = "";
        deck = 0;
        if (_gameContainer.AlreadyDrew == false && _gameContainer.PreviousCard == 0)
        {
            pickUp = true;
        }
        else
        {
            pickUp = false;
        }
        int Selected = _model!.PlayerHand1!.ObjectSelected();
        if (pickUp == true)
        {
            if (Selected > 0)
            {
                message = "There is at least one card selected.  Unselect all the cards before picking up from discard";
                return false;
            }
            return true;
        }
        if (Selected == 0)
        {
            message = "Sorry, you must select a card to discard";
            return false;
        }
        if (Selected == _gameContainer.PreviousCard)
        {
            deck = 0;
            message = "Sorry, you cannot discard the one that was picked up since there is more than one card to choose from";
            return false;
        }
        deck = Selected;
        return true;
    }
    public bool CanGoOut()
    {
        WasGuarantee = SingleInfo!.MainHandList.GuaranteedVictory(this);
        if (WasGuarantee == true)
        {
            CardForDiscard = LastCard;
            return true;
        }
        if (SingleInfo.MainHandList.HasRummy(this) == false)
        {
            return false;
        }
        CardForDiscard = LastCard;
        return true;
    }
    private async Task GameOverAsync()
    {
        SaveRoot!.RoundList.Clear();
        await ShowWinAsync();
    }
    public async Task GoOutAsync()
    {
        if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
        {
            _toast.ShowInfoToast($"{SingleInfo.NickName} went out");
        }
        else
        {
            _model!.PlayerHand1!.EndTurn();
        }
        var thisList = SingleInfo.MainHandList.ToRegularDeckDict();
        thisList.RemoveObjectByDeck(CardForDiscard!.Deck);
        await thisList.ForEachAsync(async thisCard =>
        {
            thisCard.Drew = false;
            thisCard.IsSelected = false;
            SingleInfo.MainHandList.RemoveObjectByDeck(thisCard.Deck);
            await AnimatePlayAsync(thisCard);
            if (Test!.NoAnimations == false)
            {
                await Delay!.DelaySeconds(.25);
            }
        });
        SingleInfo.MainHandList.RemoveObjectByDeck(CardForDiscard.Deck);
        await AnimatePlayAsync(CardForDiscard);
        if (WasGuarantee == true)
        {
            await GameOverAsync();
            return;
        }
        SingleInfo.TotalScore += SaveRoot!.RoundList.First().Points;
        if (SaveRoot.RoundList.Count == 1)
        {
            SingleInfo = PlayerList.OrderByDescending(Items => Items.TotalScore).First();
            await GameOverAsync();
            return;
        }
        _wasNew = true;
        PlayerStatus = EnumPlayerStatus.WentOut;
        if (SingleInfo.MainHandList.Count > 0)
        {
            throw new CustomBasicException("Can't have any cards.  Maybe trying clearing to double check but not sure");
        }
        LeftToDraw = 7;
        await DrawAsync();
    }
    protected override async Task AfterDrawingAsync()
    {
        _wasNew = false;
        if (SingleInfo!.MainHandList.Count > 8)
        {
            throw new CustomBasicException("Can't have more than 8 cards in hand");
        }
        if (PlayerStatus == EnumPlayerStatus.WentOut)
        {
            PlayerStatus = EnumPlayerStatus.Regular;
            SaveRoot!.RoundList.RemoveFirstItem();
            await EndTurnAsync();
            return;
        }
        if (PlayerStatus == EnumPlayerStatus.Reversed)
        {
            SaveRoot!.PlayOrder.IsReversed = !SaveRoot.PlayOrder.IsReversed;
            if (SaveRoot.PlayOrder.OtherTurn == 0)
            {
                throw new CustomBasicException("Otherturn must be filled in");
            }
            WhoTurn = SaveRoot.PlayOrder.OtherTurn;
            SaveRoot.PlayOrder.OtherTurn = 0;
            PlayerStatus = EnumPlayerStatus.Regular;
            await StartNewTurnAsync();
            return;
        }
        _gameContainer.AlreadyDrew = true;
        await base.AfterDrawingAsync();
    }
    public override async Task DiscardAsync(A8RoundRummyCardInformation thisCard)
    {
        if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
        {
            _model!.PlayerHand1!.EndTurn();
        }
        if (thisCard.CardType == EnumCardType.Reverse)
        {
            thisCard.Drew = false;
            thisCard.IsSelected = false;
            if (SingleInfo.PlayerCategory != EnumPlayerCategory.Computer)
            {
                SingleInfo.MainHandList.RemoveObjectByDeck(thisCard.Deck);
            }
            bool ShowMessageBox;
            await AnimatePlayAsync(thisCard);
            ShowMessageBox = SingleInfo.PlayerCategory != EnumPlayerCategory.Self;
            SaveRoot!.PlayOrder.OtherTurn = WhoTurn; //after being reversed then the current player will get another turn.
            PlayerStatus = EnumPlayerStatus.Reversed;
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            SingleInfo = PlayerList.GetWhoPlayer();
            if (ShowMessageBox == true)
            {
                _toast.ShowWarningToast($"{SingleInfo.NickName} has to discard all the cards and draw new cards");
            }
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.OtherHuman)
            {
                Network!.IsEnabled = true;
                return; //waiting for message from other player.
            }
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Computer && BasicData.MultiPlayer)
            {
                throw new CustomBasicException("This game should not have had any extra computer players");
            }
            var list = SingleInfo.MainHandList.GetRandomList().ToRegularDeckDict(); //hopefully gets full list
            if (list.Count != SingleInfo.MainHandList.Count)
            {
                throw new CustomBasicException("After getting random cards, does not reconcile the count.  This means missing cards");
            }
            if (BasicData.MultiPlayer)
            {
                await Network!.SendCustomDeckListAsync("reversecards", list);
            }
            await DiscardAllCardsAsync(list);
            return;
        }
        if (SingleInfo.PlayerCategory != EnumPlayerCategory.Computer)
        {
            SingleInfo.MainHandList.RemoveObjectByDeck(thisCard.Deck);
            if (SingleInfo.MainHandList.Count == 8)
            {
                throw new CustomBasicException("Failed To Discard");
            }
        }
        await AnimatePlayAsync(thisCard);
        await EndTurnAsync();
    }
    private async Task DiscardAllCardsAsync(DeckRegularDict<A8RoundRummyCardInformation> cards)
    {
        //the player whose turn it is has to already know what cards to send to other players to discard.
        await cards.ForEachAsync(async card =>
        {
            SingleInfo!.MainHandList.RemoveObjectByDeck(card.Deck);
            await AnimatePlayAsync(card);
        });
        LeftToDraw = 7;
        _wasNew = true;
        PlayerDraws = WhoTurn;
        await DrawAsync();
    }
}