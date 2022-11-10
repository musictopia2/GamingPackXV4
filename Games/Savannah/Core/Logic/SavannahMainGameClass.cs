namespace Savannah.Core.Logic;
[SingletonGame]
public class SavannahMainGameClass
    : CardGameClass<RegularSimpleCard, SavannahPlayerItem, SavannahSaveInfo>
    , IMiscDataNM, ISerializable
{
    private readonly SavannahVMData _model;
    private readonly CommandContainer _command;
    public StandardRollProcesses<SimpleDice, SavannahPlayerItem> Roller;
    private readonly SavannahGameContainer _gameContainer;
    private bool _wasNew;
    private bool _willClearBoard;
    private bool _startTurn;
    private DeckRegularDict<RegularSimpleCard> _pileCards = new();
    public SavannahMainGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        SavannahVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<RegularSimpleCard> cardInfo,
        CommandContainer command,
        SavannahGameContainer gameContainer,
        StandardRollProcesses<SimpleDice, SavannahPlayerItem> roller,
        ISystemError error,
        IToast toast
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, error, toast)
    {
        _model = currentMod;
        _command = command;
        Roller = roller;
        Roller.AfterRollingAsync = AfterRollingAsync;
        Roller.CurrentPlayer = () => SingleInfo!;
        _gameContainer = gameContainer;
        _gameContainer.UnselectAllPilesAsync = UnselectAllPilesAsync;
        _gameContainer.DiscardAsync = DiscardToSelfAsync;
    }
    public override Task FinishGetSavedAsync()
    {
        LoadControls();
        _model!.LoadCup(SaveRoot, true);
        foreach (var player in PlayerList)
        {
            player.SelfDiscard = new(_command, player, _gameContainer);
            player.SelfDiscard.Reload();
        }
        _model.PublicPiles!.PileList!.ReplaceRange(SaveRoot.PublicPileList);
        _model.PublicPiles.FixPiles();
        var self = PlayerList.GetSelf();
        _model.SelfDiscard = self.SelfDiscard;
        LoadPlayerStockPiles(self);
        //anything else needed is here.
        return base.FinishGetSavedAsync();
    }
    private void LoadPlayerStockPiles(SavannahPlayerItem player)
    {
        _model.SelfStock.ClearCards(); //clear out because its being reloaded.
        player.ReserveList.ForEach(card =>
        {
            _model.SelfStock.AddCard(card);
        });
    }
    public override Task PopulateSaveRootAsync()
    {
        SaveRoot!.PublicPileList = _model.PublicPiles.PileList!.ToBasicList();
        return base.PopulateSaveRootAsync();
    }
    private void LoadControls()
    {
        if (IsLoaded == true)
        {
            return;
        }

        IsLoaded = true; //i think needs to be here.
    }
    protected override async Task ComputerTurnAsync()
    {
        //if there is nothing, then just won't do anything.
        await Task.CompletedTask;
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        if (IsLoaded == false)
        {
            _model!.LoadCup(SaveRoot, false);
        }
        LoadControls();
        SaveRoot!.ImmediatelyStartTurn = true;
        return base.StartSetUpAsync(isBeginning);
    }
    private int CardsLeftForDiscard()
    {
        int count = 3;
        foreach (var player in PlayerList)
        {
            count += player.MainHandList.Count;
            count += player.DiscardList.Count;
            count += player.ReserveList.Count;
        }
        int output = _gameContainer.DeckCount - count;
        return output;
    }
    public bool EnoughCards()
    {
        int lefts = CardsLeftForDiscard();
        if (lefts == 0)
        {
            return false;
        }
        if (lefts >= 4)
        {
            return true;
        }
        return false; //because we have more than 2 players now.
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status) //can't do switch because we don't know what the cases are ahead of time.
        {
            //put in cases here.
            case nameof(IMultiplayerModel.DiscardPile):
                await DiscardToSelfAsync(int.Parse(content));
                break;
            case nameof(IMultiplayerModel.Play):
                SendPlay play = await js.DeserializeObjectAsync<SendPlay>(content);
                await PlayOnPileAsync(play);
                break;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override async Task StartNewTurnAsync()
    {
        await base.StartNewTurnAsync();
        if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Computer)
        {
            await EndTurnAsync(); //has to end turn here instead of continueturn so it will not even roll for the computer player.
            return;
        }
        bool rets = IsBlocked();
        if (rets)
        {
            _startTurn = true;
            await RefreshBoardAsync();
            return;
        }
        await FinishStartTurnAsync();
    }
    private async Task FinishStartTurnAsync()
    {
        _startTurn = false;
        if (SingleInfo!.PlayerCategory == EnumPlayerCategory.OtherHuman)
        {
            Network!.IsEnabled = true; //waiting.  has to come from other players for rolling dice.
            return;
        }
        SaveRoot.ChoseOtherPlayer = false;
        _model.Cup!.ClearDice(); //i think.
        await Roller!.RollDiceAsync(); //does automatically here
    }
    public override async Task ContinueTurnAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        if (SingleInfo.MainHandList.Count == 0)
        {
            await StartDrawingAsync();
            return;
        }
        await base.ContinueTurnAsync();
    }
    protected override Task LastPartOfSetUpBeforeBindingsAsync()
    {
        PlayerList.ForEach(player =>
        {
            var list = player.MainHandList.ToBasicList();
            player.WhenToStackDiscards = 5; //0 based.
            player.MainHandList.Clear();
            player.ReserveList.ReplaceRange(list.Take(13));
            player.DiscardList.ReplaceRange(list.Skip(13).Take(6));
            player.SelfDiscard = new(_command, player, _gameContainer);
            player.SelfDiscard.ClearBoard();
            if (player.PlayerCategory == EnumPlayerCategory.Self)
            {
                LoadPlayerStockPiles(player);
                _model.SelfDiscard = player.SelfDiscard;
            }
        });
        var fins = _model.Deck1.DrawSeveralCards(3);
        _model.PublicPiles.ClearBoard(fins);
        SaveRoot.TimesReshuffled = 0;
        return base.LastPartOfSetUpBeforeBindingsAsync();
    }
    public override async Task EndTurnAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.
        if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
        {
            _model.PlayerHand1.EndTurn();
            _model.SelfDiscard!.EndTurn();
        }
        _command.ManuelFinish = true; //because it could be somebody else's turn.
        WhoTurn = await PlayerList.CalculateWhoTurnAsync();
        await StartNewTurnAsync();
    }
    public async Task AfterRollingAsync()
    {
        await StartDrawingAsync();
    }
    protected override bool CanReshuffleAgain => SaveRoot.TimesReshuffled < 2;
    protected override async Task NoReshuffleAgainAsync()
    {
        //this means has to end game early.
        int leastCards = 1000000;
        foreach (var player in PlayerList)
        {
            int totals = player.DiscardLeft + player.ReserveLeft;
            if (totals < leastCards)
            {
                SingleInfo = player;
                leastCards = totals; //hopefully this will fix the bug
            }
        }
        await ShowWinAsync();
    }
    private async Task StartDrawingAsync()
    {
        int needs = 4 - SingleInfo!.MainHandList.Count;
        int lefts = CardsLeftForDiscard();
        if (lefts == 0)
        {
            throw new CustomBasicException("Cannot be 0 cards left.  Find out what happened");
        }
        if (lefts < needs)
        {
            LeftToDraw = lefts; //there is somehow not enough cards no matter what.
        }
        else
        {
            LeftToDraw = needs;
        }
        _wasNew = SingleInfo.MainHandList.Count == 0;
        PlayerDraws = WhoTurn;
        _willClearBoard = false;
        await DrawAsync();
    }
    protected override async Task AddCardAsync(RegularSimpleCard thisCard, SavannahPlayerItem tempPlayer)
    {
        if (_willClearBoard)
        {
            _pileCards.Add(thisCard);
        }
        else
        {
            await base.AddCardAsync(thisCard, tempPlayer);
        }
    }
    protected override Task AfterReshuffleAsync()
    {
        SaveRoot.TimesReshuffled++;
        return base.AfterReshuffleAsync();
    }
    protected override async Task AfterDrawingAsync()
    {
        if (_wasNew && _willClearBoard)
        {
            throw new CustomBasicException("I don't think we can clear board and show all new cards");
        }
        if (_wasNew == true)
        {
            SingleInfo!.MainHandList.UnhighlightObjects();
            SortCards(); //i think.
            _wasNew = false;
        }
        if (_willClearBoard)
        {
            if (_pileCards.Count != 3)
            {
                throw new CustomBasicException("Should have drawn 3 cards to clear the board for public piles");
            }
            _model.PublicPiles.ClearBoard(_pileCards);
            _pileCards.Clear();
            _willClearBoard = false; //not anymore.
        }
        if (_startTurn)
        {
            await FinishStartTurnAsync();
            return;
        }
        await base.AfterDrawingAsync();
    }
    private async Task UnselectAllPilesAsync()
    {
        await Task.Delay(0); //not sure if we needed async.  do just in case
        _model.PlayerHand1.UnselectAllObjects();
        foreach (var player in PlayerList)
        {
            if (player.DiscardList.Count > 0)
            {
                player.DiscardList.Last().IsSelected = false; //just in case
            }
        }
        _model.Pile1.UnselectCard();
        _model.SelfStock.UnselectCard();
    }
    private DeckRegularDict<RegularSimpleCard> RemainingCards()
    {
        DeckRegularDict<RegularSimpleCard> output = new();
        foreach (var player in PlayerList)
        {
            output.AddRange(player.DiscardList.ToBasicList());
            output.AddRange(player.ReserveList.ToBasicList()); //hopefully its this simple (?)
            output.AddRange(player.MainHandList.ToBasicList());
        }
        for (int i = 0; i < 3; i++)
        {
            output.Add(_model.PublicPiles.GetLastCard(i));
        }
        return output;
    }
    protected override DeckRegularDict<RegularSimpleCard> GetReshuffleList()
    {
        int maxs = _gameContainer.DeckCount;
        DeckRegularDict<RegularSimpleCard> remains = RemainingCards();
        DeckRegularDict<RegularSimpleCard> output = new();
        for (int i = 1; i <= maxs; i++)
        {
            if (remains.ObjectExist(i) == false)
            {
                RegularSimpleCard card = new();
                card.Populate(i);
                output.Add(card);
            }
        }
        return output;
    }
    private async Task DiscardToSelfAsync()
    {
        int deck = _model.PlayerHand1.ObjectSelected();
        await DiscardToSelfAsync(deck);
    }
    private async Task DiscardToSelfAsync(int deck)
    {
        if (_gameContainer.CanSendMessage())
        {
            await Network!.SendAllAsync(nameof(IMultiplayerModel.DiscardPile), deck);
        }
        RegularSimpleCard card = SingleInfo!.MainHandList.GetSpecificItem(deck);
        card.IsSelected = false;
        SingleInfo.MainHandList.RemoveSpecificItem(card);
        SingleInfo.DiscardList.Add(card);
        _command.UpdateAll();
        await EndTurnAsync(); //no need for animations for this.
    }
    public override async Task DiscardAsync(RegularSimpleCard thisCard)
    {
        SingleInfo!.MainHandList.RemoveSpecificItem(thisCard); //for now, when you discard, still has to be this way.  may decide to modify (?)
        await AnimatePlayAsync(thisCard);
        await EndTurnAsync();
    }
    public SendPlay CardSelected(Action<string> message)
    {
        SendPlay output = new();
        int handSelected = _model.PlayerHand1.ObjectSelected();
        int discardSelected = 0;
        int player = 0;
        foreach (var item in PlayerList)
        {
            if (item.DiscardList.Count == 0)
            {
                continue; //try this way (?)
            }
            var lasts = item.DiscardList.Last();
            if (lasts.IsSelected)
            {
                if (discardSelected > 0)
                {
                    message.Invoke("You can only choose from one player");
                    return output;
                }
                discardSelected = lasts.Deck;
                player = item.Id;
            }
        }
        int reserveSelected = _model.SelfStock.CardSelected();
        if (handSelected == 0 && discardSelected == 0 && reserveSelected == 0)
        {
            message.Invoke("You must choose a card to play");
            return output;
        }
        BasicList<int> temps = new() { reserveSelected, discardSelected, handSelected };
        temps.RemoveAllOnly(x => x == 0);
        if (temps.Count > 1)
        {
            message.Invoke("You can choose only one card from one pile type");
            return output;
        }
        if (temps.Count == 0)
        {
            throw new CustomBasicException("You should have already accounted for requiring a card to play");
        }
        output.Deck = temps.Single();
        if (handSelected > 0)
        {
            output.WhichType = EnumSelectType.FromHand;
        }
        else if (reserveSelected > 0)
        {
            output.WhichType = EnumSelectType.FromReserve;
        }
        else if (discardSelected > 0)
        {
            output.Player = player;
            output.WhichType = EnumSelectType.FromDiscard;
        }
        else
        {
            throw new CustomBasicException("Cannot figure out what type this came from");
        }
        return output;
    }
    private bool DidGoOut()
    {
        return SingleInfo!.ReserveList.Count == 0 && SingleInfo.DiscardList.Count == 0;
    }
    public async Task PlayOnPileAsync(SendPlay play)
    {
        RegularSimpleCard card = _gameContainer.DeckList.GetSpecificItem(play.Deck);
        if (play.WhichType == EnumSelectType.FromHand)
        {
            SingleInfo!.MainHandList.RemoveObjectByDeck(play.Deck); //do this way.
        }
        else if (play.WhichType == EnumSelectType.FromReserve)
        {
            SingleInfo!.ReserveList.RemoveObjectByDeck(play.Deck);
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
            {
                _model.SelfStock.RemoveCard(); //try this (?)
            }
        }
        else if (play.WhichType == EnumSelectType.FromDiscard)
        {
            if (play.Player != WhoTurn)
            {
                SaveRoot.ChoseOtherPlayer = true; //this means you chose other player.
            }
            var player = PlayerList[play.Player];
            player.SelfDiscard!.RemoveCard();
        }
        else
        {
            throw new CustomBasicException("No Section");
        }
        card.IsSelected = false;
        card.Drew = false;
        card.IsUnknown = false;
        var thisPile = _model.PublicPiles.PileList![play.Pile];
        await Aggregator.AnimateCardAsync(card, EnumAnimcationDirection.StartDownToCard, "public", thisPile);
        _model.PublicPiles.AddCardToPile(play.Pile, card); //hopefully this is still okay.
        _command.UpdateAll();
        if (DidGoOut())
        {
            await ShowWinAsync();
            return;
        }
        if (_model.PublicPiles.CanClearBoard())
        {
            //this means needs 3 new cards.
            await RefreshBoardAsync();
            return;
        }
        if (IsBlocked())
        {
            await RefreshBoardAsync();
            return;
        }
        await ContinueTurnAsync();
    }
    private async Task RefreshBoardAsync()
    {
        _willClearBoard = true;
        LeftToDraw = 3;
        _pileCards.Clear();
        await DrawAsync();
    }
    private bool IsBlocked()
    {
        if (_gameContainer.Test.AllowAnyMove)
        {
            return false;
        }
        var newList = GetReshuffleList();
        foreach (var player in PlayerList)
        {
            newList.AddRange(player.MainHandList);
            if (player.ReserveList.Count > 0)
            {
                newList.Add(player.ReserveList.Last());
            }
            if (player.DiscardList.Count > 0)
            {
                newList.Add(player.DiscardList.Last());
            }
        }
        bool rets;
        foreach (var card in newList)
        {
            for (int i = 0; i < 3; i++)
            {
                rets = _model.PublicPiles.CanPlayOnPile(i, 0, card);
                if (rets == true)
                {
                    return false;
                }
            }
        }
        return true;
    }
}