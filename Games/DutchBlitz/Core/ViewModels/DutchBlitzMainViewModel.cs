namespace DutchBlitz.Core.ViewModels;
[InstanceGame]
public partial class DutchBlitzMainViewModel : BasicCardGamesVM<DutchBlitzCardInformation>
{
    private readonly DutchBlitzMainGameClass _mainGame;
    private readonly DutchBlitzVMData _model;
    private readonly DutchBlitzGameContainer _gameContainer;
    public DutchBlitzMainViewModel(CommandContainer commandContainer,
        DutchBlitzMainGameClass mainGame,
        DutchBlitzVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        DutchBlitzGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _mainGame = mainGame;
        _model = viewModel;
        _gameContainer = gameContainer;
        _model.Deck1.NeverAutoDisable = true;
        CanSendDrawMessage = false;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    protected override Task ActivateAsync()
    {
        if (_model.DiscardPiles == null)
        {
            throw new CustomBasicException("No discard piles.  Rethink");
        }
        _model.DiscardPiles.PileClickedAsync += DiscardPiles_PileClickedAsync;
        _model.StockPile.StockFrame.SendEnableProcesses(this, () => _model.StockPile.CardsLeft() > 0);
        _model.StockPile.StockClickedAsync += StockPile_StockClickedAsync;
        _model.PublicPiles1.PileClickedAsync += PublicPiles1_PileClickedAsync;
        _model.PublicPiles1.NewPileClickedAsync += PublicPiles1_NewPileClickedAsync;
        CommandContainer.ExecutingChanged += CommandContainer_ExecutingChanged;
        _model.Stops.TimeUp += Stops_TimeUp;
        return base.ActivateAsync();
    }
    private void CommandContainer_ExecutingChanged()
    {
        if (CommandContainer!.IsExecuting)
        {
            if (_model.DidStartTimer == true)
            {
                _model.Stops!.PauseTimer();
            }
        }
        else
        {
            if (_model.DidStartTimer)
            {
                _model.Stops!.ContinueTimer();
            }
        }
    }
    private async void Stops_TimeUp()
    {
        if (_model.DidStartTimer == false)
        {
            return;
        }
        _model.DidStartTimer = false;
        CommandContainer!.IsExecuting = true;
        CommandContainer.ManuelFinish = true;
        if (_mainGame.BasicData!.MultiPlayer == true)
        {
            await _mainGame.Network!.SendEndTurnAsync();
        }
        await _mainGame!.EndTurnAsync();
    }
    protected override Task TryCloseAsync()
    {
        _model.DiscardPiles!.PileClickedAsync -= DiscardPiles_PileClickedAsync;
        _model.StockPile.StockClickedAsync -= StockPile_StockClickedAsync;
        _model.PublicPiles1.PileClickedAsync -= PublicPiles1_PileClickedAsync;
        _model.PublicPiles1.NewPileClickedAsync -= PublicPiles1_NewPileClickedAsync;
        CommandContainer.ExecutingChanged -= CommandContainer_ExecutingChanged;
        _model.Stops.TimeUp -= Stops_TimeUp;
        return base.TryCloseAsync();
    }
    private bool _didClickOld = false;
    private async Task PublicPiles1_NewPileClickedAsync()
    {
        if (_didClickOld == true)
        {
            _didClickOld = false;
            return;
        }
        await ProcessHumanPileAsync(-1);
    }
    private async Task PublicPiles1_PileClickedAsync(int index)
    {
        _didClickOld = true;
        await ProcessHumanPileAsync(index);
    }
    private async Task StockPile_StockClickedAsync()
    {
        int nums = _model.StockPile!.CardSelected();
        if (nums > 0)
        {
            _model.StockPile.UnselectCard();
            return;
        }
        _mainGame!.UnselectCards();
        _model.StockPile.SelectCard();
        CommandContainer.UpdateAll();
        await Task.CompletedTask;
    }
    private async Task DiscardPiles_PileClickedAsync(int index, BasicPileInfo<DutchBlitzCardInformation> thisPile)
    {
        DutchBlitzCardInformation thisCard;
        if (_model.DiscardPiles!.HasCard(index) == false)
        {
            if (_model.StockPile!.CardSelected() > 0)
            {
                thisCard = _model.StockPile.GetCard();
                _model.StockPile.RemoveCard();
                _model.DiscardPiles.AddToEmptyDiscard(thisCard);
                _mainGame!.SingleInfo!.StockLeft--;
                await _mainGame.SendStockAsync();
                return;
            }
            if (_model.Pile1!.CardSelected() == 0)
            {
                await SendErrorMessageAsync("Sorry, no card was selected");
                return;
            }
            thisCard = _model.Pile1.GetCardInfo();
            thisCard.IsSelected = false;
            thisCard.Drew = false;
            _model.DiscardPiles.AddToEmptyDiscard(thisCard);
            _model.Pile1.RemoveFromPile();
            return;
        }
        thisCard = _model.DiscardPiles.GetLastCard(index);
        if (thisCard.IsSelected)
        {
            _model.DiscardPiles.SelectUnselectSinglePile(index);
            return;
        }
        _mainGame!.UnselectCards();
        _model.DiscardPiles.SelectUnselectSinglePile(index);
    }
    protected override bool CanEnableDeck()
    {
        if (_model.Deck1!.IsEndOfDeck() && _model.Pile1!.DiscardList().Count > 1)
        {
            return false;
        }
        return true;
    }
    protected override bool CanEnablePile1()
    {
        return !_model.Pile1!.PileEmpty();
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        await Task.CompletedTask;
        if (_model.Pile1!.GetCardInfo().Deck == 0)
        {
            return;
        }
        if (_model.Pile1.CardSelected() > 0)
        {
            _model.Pile1.IsSelected(false);
            return;
        }
        _mainGame!.UnselectCards();
        _model.Pile1.IsSelected(true);
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    private async Task SendErrorMessageAsync(string message)
    {
        _model.ErrorMessage = message;
        if (_gameContainer.Test!.NoAnimations == false)
        {
            await _gameContainer!.Delay!.DelayMilli(200);
        }
        _model.ErrorMessage = "";
    }
    private async Task ProcessHumanPileAsync(int pile)
    {
        if (_mainGame!.HumanHasSelected() == false)
        {
            await SendErrorMessageAsync("No card was selected");
            return;
        }
        if (_mainGame.CanHumanPlayCard(pile) == false)
        {
            await SendErrorMessageAsync("Illegal move");
            return;
        }
        DutchBlitzCardInformation thisCard;
        thisCard = _mainGame.HumanCardToUse(out bool _, out bool sends);
        if (sends)
        {
            await _mainGame.SendStockAsync();
        }
        if (pile == -1)
        {
            if (_mainGame.BasicData!.MultiPlayer)
                await _mainGame.Network!.SendAllAsync("newpile", thisCard.Deck);
            await _mainGame.AddNewPublicPileAsync(thisCard);
            return;
        }
        if (_mainGame.BasicData!.MultiPlayer)
        {
            SendExpand temps = new();
            temps.Deck = thisCard.Deck;
            temps.Pile = pile;
            await _mainGame.Network!.SendAllAsync("expandpile", temps);
        }
        await _mainGame.ExpandPublicPileAsync(thisCard, pile);
    }
    [Command(EnumCommandCategory.Game)]
    public async Task DutchAsync()
    {
        if (_mainGame!.HasBlitz == false)
        {
            await SendErrorMessageAsync("Sorry, you still have cards in your stock pile.  Therefore, the round has not ended yet");
            return;
        }
        if (_mainGame.BasicData!.MultiPlayer)
        {
            await _mainGame.Network!.SendAllAsync("blitz");
        }
        _model.Stops.PauseTimer();
        _model.DidStartTimer = false; //because you got blitz.
        await _mainGame.BlitzAsync();
    }
}