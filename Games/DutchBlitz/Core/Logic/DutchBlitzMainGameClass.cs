namespace DutchBlitz.Core.Logic;
[SingletonGame]
public class DutchBlitzMainGameClass
    : CardGameClass<DutchBlitzCardInformation, DutchBlitzPlayerItem, DutchBlitzSaveInfo>
    , IMiscDataNM, IStartNewGame, ISerializable
{
    private readonly DutchBlitzVMData _model;
    private readonly CommandContainer _command;
    private readonly DutchBlitzGameContainer _gameContainer;
    private readonly IToast _toast;
    private ComputerAI? _ai;
    public DutchBlitzMainGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        DutchBlitzVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<DutchBlitzCardInformation> cardInfo,
        CommandContainer command,
        DutchBlitzGameContainer gameContainer,
        ISystemError error,
        IToast toast
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, error, toast)
    {
        _model = currentMod;
        _command = command;
        _gameContainer = gameContainer;
        _toast = toast;
    }
    private DeckRegularDict<DutchBlitzCardInformation> _otherList = new();
    public override Task FinishGetSavedAsync()
    {
        LoadControls();
        ShuffleOwnCards();
        return Task.CompletedTask;
    }
    private void LoadControls()
    {
        _otherList = new DeckRegularDict<DutchBlitzCardInformation>();
        if (IsLoaded == true)
        {
            return;
        }
        _model.LoadDiscards();
        IsLoaded = true;
    }
    protected override async Task ComputerTurnAsync()
    {
        if (HasBlitz)
        {
            await BlitzAsync();
            return;
        }
        if (_ai!.CanEndTurn)
        {
            await EndTurnAsync();
            return;
        }
        var thisMove = _ai.ComputerMove();
        var thisType = ComputerAI.CalculateMoveType(thisMove);
        if (thisType == ComputerAI.EnumMoveType.ContinueMove)
        {
            _ai.DrawCards();
            await ContinueTurnAsync();
            return;
        }
        if (thisType == ComputerAI.EnumMoveType.Transfer)
        {
            _ai.TransferCards();
            await ContinueTurnAsync();
            return;
        }
        var thisCard = _ai.CardToUseForPublic(thisMove);
        if (thisMove.NewPublicPile)
        {
            if (thisCard.CardValue != 1)
            {
                throw new CustomBasicException("Cannot create a new pile because the number is not 1");
            }
            await AddNewPublicPileAsync(thisCard);
            return;
        }
        if (thisMove.PublicPile == -1)
        {
            throw new CustomBasicException("Cannot expand on a public pile if its -1");
        }
        await ExpandPublicPileAsync(thisCard, thisMove.PublicPile);
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        LoadControls();
        SaveRoot!.ImmediatelyStartTurn = true;
        ShuffleOwnCards();
        return FinishUpAsync!.Invoke(isBeginning);
    }
    private void ShuffleOwnCards()
    {
        int whatPlayer = PlayerList!.GetSelf().Id;
        int minNum;
        int maxNum;
        if (PlayerList.Count > 2)
        {
            DutchBlitzDeckCount.DoubleDeck = false;
            switch (whatPlayer)
            {
                case 1:
                    minNum = 1;
                    maxNum = 40;
                    break;
                case 2:
                    minNum = 41;
                    maxNum = 80;
                    break;
                case 3:
                    minNum = 81;
                    maxNum = 120;
                    break;
                case 4:
                    minNum = 121;
                    maxNum = 160;
                    break;
                default:
                    throw new CustomBasicException("Not Supported");
            }
        }
        else
        {
            DutchBlitzDeckCount.DoubleDeck = true;
            if (whatPlayer == 1)
            {
                minNum = 1;
                maxNum = 80;
            }
            else if (whatPlayer == 2)
            {
                minNum = 81;
                maxNum = 160;
            }
            else
            {
                throw new CustomBasicException("Not Supported");
            }
        }
        _gameContainer.DeckList!.ClearObjects();
        _gameContainer.DeckList.ShuffleObjects();
        var thisList = GetOwnDutchBlitzCards(minNum, maxNum);
        _model!.PublicPiles1!.ClearBoard();
        _model.StockPile!.ClearCards();
        _model.DiscardPiles!.ClearCards();
        _model.Pile1.ClearCards();
        PlayerList.ForEach(thisPlayer => thisPlayer.StockLeft = 10);
        10.Times(x =>
        {
            _model.StockPile.AddCard(thisList.First());
            thisList.RemoveFirstItem();
        });
        _gameContainer.MaxDiscard.Times(x =>
        {
            _model.DiscardPiles.AddToDiscard(x, thisList.First());
            thisList.RemoveFirstItem();
        });
        _model!.Deck1!.OriginalList(thisList);
        if (BasicData!.MultiPlayer == false)
        {
            ComputerShuffle();
        }
    }
    private void ComputerShuffle()
    {
        int minNum;
        int maxNum;
        _gameContainer.ComputerPlayers = new();
        PlayerList!.ForEach(thisPlayer =>
        {
            int whatPlayer = thisPlayer.Id;
            if (PlayerList.Count > 2)
            {
                DutchBlitzDeckCount.DoubleDeck = false;
                switch (whatPlayer)
                {
                    case 1:
                        minNum = 1;
                        maxNum = 40;
                        break;
                    case 2:
                        minNum = 41;
                        maxNum = 80;
                        break;
                    case 3:
                        minNum = 81;
                        maxNum = 120;
                        break;
                    case 4:
                        minNum = 121;
                        maxNum = 160;
                        break;
                    default:
                        throw new CustomBasicException("Not Supported");
                }
            }
            else
            {
                DutchBlitzDeckCount.DoubleDeck = true;
                if (whatPlayer == 1)
                {
                    minNum = 1;
                    maxNum = 80;
                }
                else if (whatPlayer == 2)
                {
                    minNum = 81;
                    maxNum = 160;
                }
                else
                {
                    throw new CustomBasicException("Not Supported");
                }
            }
            ComputerCards thisComputer = new();
            thisComputer.Player = thisPlayer.Id;
            var thisList = GetOwnDutchBlitzCards(minNum, maxNum);
            thisList.ForEach(thisCard => thisCard.Player = whatPlayer);
            thisComputer.DeckList.AddRange(thisList);
            10.Times(x =>
            {
                thisComputer.StockList.Add(thisComputer.DeckList.First());
                thisComputer.DeckList.RemoveFirstItem();
            });
            _gameContainer.MaxDiscard.Times(x =>
            {
                thisComputer.Discard.Add(thisComputer.DeckList.First());
                thisComputer.DeckList.RemoveFirstItem();
            });
            _gameContainer.ComputerPlayers.Add(thisComputer);
        });
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "blitz":
                await BlitzAsync();
                return;
            case "newpile":
                var newCard = _gameContainer.DeckList!.GetSpecificItem(int.Parse(content));
                newCard.Player = WhoTurn;
                await AddNewPublicPileAsync(newCard);
                return;
            case "expandpile":
                var thisSend = await js1.DeserializeObjectAsync<SendExpand>(content);
                var exCard = _gameContainer.DeckList!.GetSpecificItem(thisSend.Deck);
                exCard.Player = WhoTurn;
                await ExpandPublicPileAsync(exCard, thisSend.Pile);
                return;
            case "stock":
                await UpdateStockAsync(int.Parse(content));
                return;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override async Task StartNewTurnAsync()
    {
        await base.StartNewTurnAsync();
        if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Computer)
        {
            _gameContainer.CurrentComputer = GetComputerPlayer();
            _ai = new ComputerAI(_gameContainer, _model);
        }
        else
        {
            _gameContainer.CurrentComputer = null;
        }
        if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
        {
            _toast.ShowInfoToast("Start Your Turn");
            _model!.Stops!.StartTimer();
            _model.DidStartTimer = true;
            await Task.Delay(5);
            _model.Stops.PauseTimer();
        }
        await ContinueTurnAsync();
    }
    public override async Task EndTurnAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        SingleInfo.MainHandList.UnhighlightObjects();
        if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
        {
            UnselectCards();
        }
        _command.ManuelFinish = true; //because it could be somebody else's turn.
        WhoTurn = await PlayerList.CalculateWhoTurnAsync();
        await StartNewTurnAsync();
    }
    public void UnselectCards()
    {
        _model!.DiscardPiles!.UnselectAllCards();
        _model.StockPile!.UnselectCard();
        _model.Pile1!.UnselectCard();
    }
    private ComputerCards GetComputerPlayer()
    {
        return _gameContainer.ComputerPlayers.Single(items => items.Player == WhoTurn);
    }
    internal DeckRegularDict<DutchBlitzCardInformation> GetOwnDutchBlitzCards(int startat, int endat)
    {
        return _gameContainer.DeckList.Where(items => items.Deck >= startat && items.Deck <= endat).ToRegularDeckDict();
    }
    private bool CanEndGame()
    {
        if (PlayerList.Any(items => items.PointsGame >= 30))
        {
            SingleInfo = PlayerList.OrderByDescending(items => items.PointsGame).First();
            return true;
        }
        return false;
    }
    public async Task UpdateStockAsync(int howMany)
    {
        SingleInfo!.StockLeft = howMany;
        await ContinueTurnAsync();
    }
    public async Task BlitzAsync()
    {
        await EndRoundAsync();
    }
    public bool HasBlitz => SingleInfo!.StockLeft == 0 || Test!.AllowAnyMove;
    private int CalculateScore(int player)
    {
        int played = _model!.PublicPiles1!.PointsPlayed(player, _otherList);
        var thisPlayer = PlayerList![player];
        int lefts = thisPlayer.StockLeft;
        int minuss = lefts * 2;
        if (played - minuss < 0)
        {
            return 0;
        }
        return played - minuss;
    }
    public override async Task EndRoundAsync()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            int points = CalculateScore(thisPlayer.Id);
            thisPlayer.PointsRound = points;
            thisPlayer.PointsGame += points;
        });
        if (CanEndGame())
        {
            await ShowWinAsync();
            return;
        }
        await this.RoundOverNextAsync();
    }
    Task IStartNewGame.ResetAsync()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.PointsGame = 0;
            thisPlayer.PointsRound = 0;
        });
        return Task.CompletedTask;
    }
    public async Task SendStockAsync()
    {
        if (BasicData!.MultiPlayer == false)
        {
            return;
        }
        await Network!.SendAllAsync("stock", SingleInfo!.StockLeft);
    }
    public async Task AddNewPublicPileAsync(DutchBlitzCardInformation thisCard)
    {
        thisCard.Player = WhoTurn;
        thisCard.Drew = false;
        thisCard.IsUnknown = false;
        thisCard.IsSelected = false;
        _model!.PublicPiles1!.CreateNewPile(thisCard);
        _command.UpdateAll();
        if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Computer && Test!.NoAnimations == false)
        {
            await Delay!.DelaySeconds(.25);
        }
        await ContinueTurnAsync();
    }
    public async Task ExpandPublicPileAsync(DutchBlitzCardInformation thisCard, int index)
    {
        thisCard.Player = WhoTurn;
        thisCard.Drew = false;
        thisCard.IsSelected = false;
        thisCard.IsUnknown = false;
        _model!.PublicPiles1!.AddCardToPile(index, thisCard);
        _command.UpdateAll();
        if (_model.PublicPiles1.NeedToRemovePile(index))
        {
            if (Test!.NoAnimations == false)
            {
                await Delay!.DelaySeconds(1);
            }
            var thisList = _model.PublicPiles1.EmptyPileList(index);
            _otherList.AddRange(thisList);
        }
        else
        {
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Computer && Test!.NoAnimations == false)
            {
                await Delay!.DelaySeconds(.25);
            }
        }
        await ContinueTurnAsync();
    }
    public bool HumanHasSelected()
    {
        if (_model!.StockPile!.CardSelected() > 0)
        {
            return true;
        }
        if (_model.Pile1!.CardSelected() > 0)
        {
            return true;
        }
        _ = _model.DiscardPiles!.CardSelected(out int pile);
        return pile > -1;
    }
    public bool CanHumanPlayCard(int index)
    {
        int decks;
        if (_model!.StockPile!.CardSelected() > 0)
        {
            decks = _model.StockPile.CardSelected();
        }
        else if (_model.Pile1!.CardSelected() > 0)
        {
            decks = _model.Pile1.GetCardInfo().Deck;
        }
        else
        {
            decks = _model.DiscardPiles!.CardSelected(out int _);
        }
        if (index == -1)
        {
            return _model.PublicPiles1!.CanCreatePile(decks);
        }
        else
        {
            return _model.PublicPiles1!.CanAddToPile(decks, index);
        }
    }
    public DutchBlitzCardInformation HumanCardToUse(out bool fromStock, out bool doSendStock)
    {
        fromStock = false;
        doSendStock = false;
        DutchBlitzCardInformation thisCard;
        if (_model!.StockPile!.CardSelected() > 0)
        {
            thisCard = _model.StockPile.GetCard();
            fromStock = true;
            doSendStock = true; //another function will have to do this (because when using byref, cannot await those)
            SingleInfo!.StockLeft--;
            _model.StockPile.RemoveCard();
            return thisCard;
        }
        if (_model.Pile1!.CardSelected() > 0)
        {
            thisCard = _model.Pile1.GetCardInfo();
            _model.Pile1.RemoveFromPile();
            return thisCard;
        }
        int decks = _model.DiscardPiles!.CardSelected(out int piles);
        if (piles == -1)
        {
            throw new CustomBasicException("No card was selected");
        }
        thisCard = _gameContainer.DeckList!.GetSpecificItem(decks);
        _model.DiscardPiles.RemoveCard(piles, decks);
        return thisCard;
    }
}