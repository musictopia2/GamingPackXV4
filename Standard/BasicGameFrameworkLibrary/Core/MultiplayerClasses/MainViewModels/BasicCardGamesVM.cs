namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.MainViewModels;
public abstract class BasicCardGamesVM<D>(CommandContainer commandContainer,
        ICardGameMainProcesses<D> mainGame,
        IBasicCardGamesData<D> viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast
        ) : BasicMultiplayerMainVM(commandContainer, mainGame, basicData, resolver, aggregator)
    where D : IDeckObject, new()
{
    protected IToast Toast { get; private set; } = toast;
    protected override void AdditionalInitialize()
    {
        viewModel.Deck1.DeckClickedAsync = Deck1_DeckClickedAsync;
        viewModel.Pile1.PileClickedAsync = ProcessDiscardClickedAsync;
        viewModel.Deck1.SendEnableProcesses(this, CanEnableDeck);
        viewModel.Pile1.SendEnableProcesses(this, CanEnablePile1);
        if (AlwaysEnableHand() == false)
        {
            viewModel.PlayerHand1.SendEnableProcesses(this, CanEnableHand);
            viewModel.PlayerHand1.IsEnabled = false;
        }
        else
        {
            viewModel.PlayerHand1.SendAlwaysEnable(this);
        }
        viewModel.PlayerHand1.Text = "Your Cards";
        viewModel.PlayerHand1.ObjectClickedAsync = ProcessHandClickedAsync;
        viewModel.PlayerHand1.ConsiderSelectOneAsync = OnConsiderSelectOneCardAsync;
        viewModel.PlayerHand1.BeforeAutoSelectObjectAsync = BeforeUnselectCardFromHandAsync;
        viewModel.PlayerHand1.AutoSelectedOneCompletedAsync = OnAutoSelectedHandAsync;
    }

    protected bool CanSendDrawMessage = true; // for games like dutch blitz, cannot send the message for drawing card.
    private async Task Deck1_DeckClickedAsync()
    {
        if (viewModel.PlayerHand1!.HasSelectedObject())
        {
            Toast.ShowUserErrorToast("You have to unselect all cards before drawing to prevent drawing by mistake");
            return;
        }
        if (BasicData.MultiPlayer == true && CanSendDrawMessage == true)
        {
            await mainGame.Network!.SendAllAsync("drawcard");
        }
        mainGame.PlayerDraws = 0;
        mainGame.LeftToDraw = 0;
        await mainGame.DrawAsync();
    }
    protected abstract bool CanEnableDeck();
    protected abstract bool CanEnablePile1();
    protected virtual bool CanEnableHand()
    {
        return false;
    }
    protected virtual bool AlwaysEnableHand()
    {
        return true;
    }
    protected abstract Task ProcessDiscardClickedAsync();
    protected virtual Task BeforeUnselectCardFromHandAsync()
    {
        return Task.CompletedTask;
    }
    protected virtual Task OnAutoSelectedHandAsync()
    {
        return Task.CompletedTask;
    }
    protected virtual Task OnConsiderSelectOneCardAsync(D payLoad)
    {
        return Task.CompletedTask;
    }
    protected virtual Task ProcessHandClickedAsync(D card, int index)
    {
        return Task.CompletedTask;
    }
}