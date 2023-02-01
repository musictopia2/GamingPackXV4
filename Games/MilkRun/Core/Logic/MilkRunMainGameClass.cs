namespace MilkRun.Core.Logic;
[SingletonGame]
public class MilkRunMainGameClass
    : CardGameClass<MilkRunCardInformation, MilkRunPlayerItem, MilkRunSaveInfo>
    , IMiscDataNM, ISerializable
{
    private readonly MilkRunVMData _model;
    private readonly CommandContainer _command;
    private readonly MilkRunGameContainer _gameContainer;
    private readonly ComputerAI _ai;
    private readonly IToast _toast;
    public MilkRunMainGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        MilkRunVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<MilkRunCardInformation> cardInfo,
        CommandContainer command,
        MilkRunGameContainer gameContainer,
        ComputerAI ai,
        ISystemError error,
        IToast toast
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, error, toast)
    {
        _model = currentMod;
        _command = command;
        _gameContainer = gameContainer;
        _ai = ai;
        _toast = toast;
        _gameContainer.CanMakeMove = CanMakeMove;
    }
    public override Task FinishGetSavedAsync()
    {
        LoadPiles();
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.LoadSavedData();
        });
        return base.FinishGetSavedAsync();
    }
    public override async Task PopulateSaveRootAsync()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.SavePileData();
        });
        await base.PopulateSaveRootAsync();
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        LoadPiles();
        SaveRoot!.DrawnFromDiscard = false;
        SaveRoot.CardsDrawn = 0;
        PlayerList!.ForEach(thisPlayer => thisPlayer.ClearBoard());
        return base.StartSetUpAsync(isBeginning);
    }
    protected override async Task ComputerTurnAsync()
    {
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelaySeconds(.5);
        }
        if (_ai.CanDraw)
        {
            await DrawAsync();
            return;
        }
        ComputerAI.MoveInfo thisMove = _ai.MoveToMake();
        if (thisMove.ToDiscard == false)
        {
            await ProcessPlayAsync(thisMove.Player, thisMove.Deck, thisMove.Pile, thisMove.Milk);
        }
        else
        {
            await DiscardAsync(thisMove.Deck);
        }
    }
    public async Task PlayerPileClickedAsync(MilkRunPlayerItem thisPlayer, PileInfo pileData)
    {
        int newDeck = _model!.PlayerHand1!.ObjectSelected();
        if (newDeck == 0)
        {
            _toast.ShowUserErrorToast("Sorry, must choose a card to play");
            return;
        }
        if (SaveRoot!.CardsDrawn < 2)
        {
            _toast.ShowUserErrorToast("Sorry, must draw the 2 cards first before playing");
            return;
        }
        int index = thisPlayer.Id;
        if (CanMakeMove(index, newDeck, pileData.Pile, pileData.Milk) == false)
        {
            _toast.ShowUserErrorToast("Illegal Move");
            return;
        }
        if (BasicData!.MultiPlayer == true)
        {
            SendPlay thisSend = new();
            thisSend.Deck = newDeck;
            thisSend.Player = index;
            thisSend.Milk = pileData.Milk;
            thisSend.Pile = pileData.Pile;
            await Network!.SendAllAsync("play", thisSend);
        }
        await ProcessPlayAsync(index, newDeck, pileData.Pile, pileData.Milk);
    }
    private void LoadPiles()
    {
        if (PlayerList.First().StrawberryPiles != null)
        {
            return;
        }
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.LoadPiles(this, _gameContainer);
        });
    }

    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "play":
                SendPlay thisSend = await js1.DeserializeObjectAsync<SendPlay>(content);
                await ProcessPlayAsync(thisSend.Player, thisSend.Deck, thisSend.Pile, thisSend.Milk);
                break;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override async Task StartNewTurnAsync()
    {
        await base.StartNewTurnAsync();
        SaveRoot!.CardsDrawn = 0;
        SaveRoot.DrawnFromDiscard = false;
        await ContinueTurnAsync();
    }
    public override async Task EndTurnAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        SingleInfo.MainHandList.UnhighlightObjects();
        int wins = WhoWins();
        if (wins > 0)
        {
            SingleInfo = PlayerList[wins];
            await ShowWinAsync();
            return;
        }
        _command.ManuelFinish = true; //because it could be somebody else's turn.
        WhoTurn = await PlayerList.CalculateWhoTurnAsync();
        await StartNewTurnAsync();
    }
    private int WhoWins()
    {
        foreach (var thisPlayer in PlayerList!)
        {
            if (thisPlayer.ReachedChocolateGoal == true && thisPlayer.ReachedStrawberryGoal == true)
            {
                return thisPlayer.Id;
            }
        }
        return 0;
    }
    protected override Task AfterDrawingAsync()
    {
        if (SaveRoot!.CardsDrawn >= 2)
        {
            throw new CustomBasicException("Can't draw more than 2 cards");
        }
        SaveRoot.CardsDrawn++;
        PlayerDraws = 0;
        return base.AfterDrawingAsync();
    }
    protected override Task AfterPickupFromDiscardAsync()
    {
        SaveRoot!.DrawnFromDiscard = true;
        if (SaveRoot.CardsDrawn >= 2)
        {
            throw new CustomBasicException("Can't draw more than 2 cards");
        }
        SaveRoot.CardsDrawn++;
        if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
        {
            SortCards();
        }
        return base.AfterPickupFromDiscardAsync();
    }
    public override async Task DiscardAsync(MilkRunCardInformation thisCard)
    {
        SingleInfo!.MainHandList.RemoveSpecificItem(thisCard);
        await AnimatePlayAsync(thisCard);
        await ContinuePlayAsync();
    }
    private async Task ContinuePlayAsync()
    {
        if (CanEndTurn() == true)
        {
            await EndTurnAsync();
        }
        else
        {
            await ContinueTurnAsync();
        }
    }
    private bool CanEndTurn()
    {
        if (SaveRoot!.CardsDrawn < 2)
        {
            return false;
        }
        if (SingleInfo!.MainHandList.Count > 8)
        {
            throw new CustomBasicException("Can't have more than 8 cards");
        }
        return (SingleInfo.MainHandList.Count == 6);
    }
    protected override DeckRegularDict<MilkRunCardInformation> GetReshuffleList()
    {
        DeckRegularDict<MilkRunCardInformation> output = new();
        var tempList = _model!.Pile1!.FlipCardList();
        int x;
        for (x = 1; x <= 3; x++)
        {
            if (x > tempList.Count)
            {
                break;
            }
            output.Add(tempList[x - 1]);
        }
        PlayerList!.ForEach(thisPlayer =>
        {
            output.AddRange(thisPlayer.GetPileCardList());
        });
        DeckRegularDict<MilkRunCardInformation> finalList = new();
        _gameContainer.DeckList!.ForEach(thisCard =>
        {
            if (output.Contains(thisCard) == false)
            {
                finalList.Add(thisCard);
            }
        });
        return finalList;
    }
    public bool CanMakeMove(int player, int deck, EnumPileType pile, EnumMilkType milk)
    {
        MilkRunPlayerItem tempPlayer = PlayerList![player];
        MilkRunCardInformation thisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
        if (thisCard.MilkCategory != milk)
        {
            return false;
        }
        if (pile == EnumPileType.Limit)
        {
            return thisCard.CardCategory == EnumCardCategory.Points;
        }
        if (pile == EnumPileType.Go && thisCard.CardCategory == EnumCardCategory.Points)
        {
            return false;
        }
        if (pile == EnumPileType.Deliveries && thisCard.CardCategory != EnumCardCategory.Points)
        {
            return false;
        }
        if (thisCard.CardCategory == EnumCardCategory.Joker && pile != EnumPileType.Go)
        {
            return false;
        }
        bool gos = tempPlayer.HasGo(milk);
        int limits;
        if (player == WhoTurn)
        {
            if (thisCard.CardCategory == EnumCardCategory.Joker)
            {
                return false;
            }
            if (pile == EnumPileType.Deliveries)
            {
                limits = tempPlayer.DeliveryLimit(milk);
                return gos == true && thisCard.Points <= limits;
            }
        }
        if (pile == EnumPileType.Deliveries)
        {
            return false;
        }
        if (pile == EnumPileType.Go)
        {
            if (thisCard.CardCategory == EnumCardCategory.Joker)
            {
                var selfPlayer = PlayerList.GetSelf();
                if (selfPlayer.HasGo(milk) == false || tempPlayer.HasGo(milk) == false)
                {
                    return false;
                }
                if (tempPlayer.HasCard(milk) == false)
                {
                    return false;
                }
                int nums = tempPlayer.LastDelivery(milk);
                return nums <= selfPlayer.DeliveryLimit(milk);
            }
            gos = tempPlayer.HasGo(milk);
            if (gos == true && thisCard.CardCategory == EnumCardCategory.Go)
            {
                return false;
            }
            if (gos == false && thisCard.CardCategory == EnumCardCategory.Stop)
            {
                return false;
            }
            return true;
        }
        throw new CustomBasicException("Cannot figure out whether the move can be made or not");
    }
    public async Task ProcessPlayAsync(int player, int deck, EnumPileType pile, EnumMilkType milk)
    {
        if (milk != EnumMilkType.Chocolate && milk != EnumMilkType.Strawberry)
        {
            throw new CustomBasicException("Must be chocolate or strawberry at the beginning of process play");
        }
        var tempPlayer = PlayerList![player];
        var thisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
        int cardsBefore = SingleInfo!.MainHandList.Count;
        SingleInfo.MainHandList.RemoveSpecificItem(thisCard);
        _command.UpdateAll();
        int cardsAfter = SingleInfo.MainHandList.Count;
        if (cardsBefore == cardsAfter)
        {
            throw new CustomBasicException("Did not remove card");
        }
        await tempPlayer.AnimatePlayAsync(thisCard, milk, pile, EnumAnimcationDirection.StartUpToCard);
        if (pile == EnumPileType.Limit)
        {
            tempPlayer.AddLimit(deck, milk);
            await ContinuePlayAsync();
            return;
        }
        if (pile == EnumPileType.Deliveries)
        {
            tempPlayer.AddToDeliveries(deck, milk);
            await ContinuePlayAsync();
            return;
        }
        if (thisCard.CardCategory != EnumCardCategory.Joker)
        {
            tempPlayer.AddGo(deck, milk);
            await ContinuePlayAsync();
            return;
        }
        await tempPlayer.AnimatePlayAsync(thisCard, milk, EnumPileType.Deliveries, EnumAnimcationDirection.StartCardToDown);
        tempPlayer.StealCard(milk, out int newDeck);
        tempPlayer.AddGo(deck, milk);
        MilkRunPlayerItem newPlayer;
        if (player == 1)
        {
            newPlayer = PlayerList[2];
        }
        else
        {
            newPlayer = PlayerList[1];
        }
        thisCard = _gameContainer.DeckList.GetSpecificItem(newDeck);
        await newPlayer.AnimatePlayAsync(thisCard, milk, EnumPileType.Deliveries, EnumAnimcationDirection.StartUpToCard);
        newPlayer.AddToDeliveries(newDeck, milk);
        await ContinuePlayAsync();
    }
}