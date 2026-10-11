namespace DutchBlitz.Core.ViewModels;
[InstanceGame]
public partial class DutchBlitzMainViewModel(CommandContainer commandContainer,
        DutchBlitzMainGameClass mainGame,
        DutchBlitzVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        DutchBlitzGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<DutchBlitzCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    protected override void AdditionalInitialize()
    {
        viewModel.Deck1.NeverAutoDisable = true;
        CanSendDrawMessage = false;
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    partial void CreateCommands(CommandContainer command);
    protected override Task OpenAsync()
    {
        if (viewModel.DiscardPiles == null)
        {
            throw new CustomBasicException("No discard piles.  Rethink");
        }
        viewModel.DiscardPiles.PileClickedAsync = DiscardPiles_PileClickedAsync;
        viewModel.StockPile.StockFrame.SendEnableProcesses(this, () => viewModel.StockPile.CardsLeft() > 0);
        viewModel.StockPile.StockClickedAsync = StockPile_StockClickedAsync;
        viewModel.PublicPiles1.PileClickedAsync = PublicPiles1_PileClickedAsync;
        viewModel.PublicPiles1.NewPileClickedAsync = PublicPiles1_NewPileClickedAsync;
        CommandContainer.ExecutingChanged = CommandContainer_ExecutingChanged;
        viewModel.Stops.TimeUp += Stops_TimeUp; //iffy.  but can't do anything about it for now though (?)

        return base.OpenAsync();
    }
    private void CommandContainer_ExecutingChanged()
    {
        if (CommandContainer!.IsExecuting)
        {
            if (viewModel.DidStartTimer == true)
            {
                viewModel.Stops!.PauseTimer();
            }
        }
        else
        {
            if (viewModel.DidStartTimer)
            {
                viewModel.Stops!.ContinueTimer();
            }
        }
    }
    private async void Stops_TimeUp()
    {
        if (viewModel.DidStartTimer == false)
        {
            return;
        }
        viewModel.DidStartTimer = false;
        CommandContainer!.IsExecuting = true;
        CommandContainer.ManuelFinish = true;
        if (mainGame.BasicData!.MultiPlayer == true)
        {
            await mainGame.Network!.SendEndTurnAsync();
        }
        await mainGame!.EndTurnAsync();
    }
    protected override Task CloseAsync()
    {
        viewModel.Stops.TimeUp -= Stops_TimeUp;
        return base.CloseAsync();
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
        int nums = viewModel.StockPile!.CardSelected();
        if (nums > 0)
        {
            viewModel.StockPile.UnselectCard();
            return;
        }
        mainGame!.UnselectCards();
        viewModel.StockPile.SelectCard();
        await Task.CompletedTask;
    }
    private async Task DiscardPiles_PileClickedAsync(int index, BasicPileInfo<DutchBlitzCardInformation> thisPile)
    {
        DutchBlitzCardInformation thisCard;
        if (viewModel.DiscardPiles!.HasCard(index) == false)
        {
            if (viewModel.StockPile!.CardSelected() > 0)
            {
                thisCard = viewModel.StockPile.GetCard();
                viewModel.StockPile.RemoveCard();
                viewModel.DiscardPiles.AddToEmptyDiscard(thisCard);
                mainGame!.SingleInfo!.StockLeft--;
                await mainGame.SendStockAsync();
                return;
            }
            if (viewModel.Pile1!.CardSelected() == 0)
            {
                await SendErrorMessageAsync("Sorry, no card was selected");
                return;
            }
            thisCard = viewModel.Pile1.GetCardInfo();
            thisCard.IsSelected = false;
            thisCard.Drew = false;
            viewModel.DiscardPiles.AddToEmptyDiscard(thisCard);
            viewModel.Pile1.RemoveFromPile();
            return;
        }
        thisCard = viewModel.DiscardPiles.GetLastCard(index);
        if (thisCard.IsSelected)
        {
            viewModel.DiscardPiles.SelectUnselectSinglePile(index);
            return;
        }
        mainGame!.UnselectCards();
        viewModel.DiscardPiles.SelectUnselectSinglePile(index);
    }
    protected override bool CanEnableDeck()
    {
        if (viewModel.Deck1!.IsEndOfDeck() && viewModel.Pile1!.DiscardList().Count > 1)
        {
            return false;
        }
        return true;
    }
    protected override bool CanEnablePile1()
    {
        return !viewModel.Pile1!.PileEmpty();
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        await Task.CompletedTask;
        if (viewModel.Pile1!.GetCardInfo().Deck == 0)
        {
            return;
        }
        if (viewModel.Pile1.CardSelected() > 0)
        {
            viewModel.Pile1.IsSelected(false);
            return;
        }
        mainGame!.UnselectCards();
        viewModel.Pile1.IsSelected(true);
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    private async Task SendErrorMessageAsync(string message)
    {
        viewModel.ErrorMessage = message;
        if (gameContainer.Test!.NoAnimations == false)
        {
            await gameContainer!.Delay!.DelayMilli(200);
        }
        viewModel.ErrorMessage = "";
    }
    private async Task ProcessHumanPileAsync(int pile)
    {
        if (mainGame!.HumanHasSelected() == false)
        {
            await SendErrorMessageAsync("No card was selected");
            return;
        }
        if (mainGame.CanHumanPlayCard(pile) == false)
        {
            await SendErrorMessageAsync("Illegal move");
            return;
        }
        DutchBlitzCardInformation thisCard;
        thisCard = mainGame.HumanCardToUse(out bool _, out bool sends);
        if (sends)
        {
            await mainGame.SendStockAsync();
        }
        if (pile == -1)
        {
            if (mainGame.BasicData!.MultiPlayer)
            {
                await mainGame.Network!.SendAllAsync("newpile", thisCard.Deck);
            }
            await mainGame.AddNewPublicPileAsync(thisCard);
            return;
        }
        if (mainGame.BasicData!.MultiPlayer)
        {
            SendExpand temps = new();
            temps.Deck = thisCard.Deck;
            temps.Pile = pile;
            await mainGame.Network!.SendAllAsync("expandpile", temps);
        }
        await mainGame.ExpandPublicPileAsync(thisCard, pile);
    }
    [Command(EnumCommandCategory.Game)]
    public async Task DutchAsync()
    {
        if (mainGame!.HasBlitz == false)
        {
            await SendErrorMessageAsync("Sorry, you still have cards in your stock pile.  Therefore, the round has not ended yet");
            return;
        }
        if (mainGame.BasicData!.MultiPlayer)
        {
            await mainGame.Network!.SendAllAsync("blitz");
        }
        viewModel.Stops.PauseTimer();
        viewModel.DidStartTimer = false; //because you got blitz.
        await mainGame.BlitzAsync();
    }
}