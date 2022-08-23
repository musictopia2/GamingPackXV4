namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.MainViewModels;
public abstract class BasicCardGamesVM<D> : BasicMultiplayerMainVM
    where D : IDeckObject, new()
{
    private readonly ICardGameMainProcesses<D> _mainGame;
    private readonly IBasicCardGamesData<D> _model;
    private readonly BasicData _basicData;
    private readonly IToast _toast;
    public BasicCardGamesVM(CommandContainer commandContainer,
        ICardGameMainProcesses<D> mainGame,
        IBasicCardGamesData<D> viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast
        ) : base(commandContainer, mainGame, basicData, test, resolver, aggregator)
    {
        _mainGame = mainGame;
        _model = viewModel;
        _basicData = basicData;
        _toast = toast;
        _model.Deck1.DeckClickedAsync += Deck1_DeckClickedAsync;
        _model.Pile1.PileClickedAsync += ProcessDiscardClickedAsync;
        _model.Deck1.SendEnableProcesses(this, () => CanEnableDeck());
        _model.Pile1.SendEnableProcesses(this, () => CanEnablePile1());
        if (AlwaysEnableHand() == false)
        {
            _model.PlayerHand1.SendEnableProcesses(this, () =>
            {
                return CanEnableHand();
            });
            _model.PlayerHand1.IsEnabled = false; // start with false
        }
        else
        {
            _model.PlayerHand1.SendAlwaysEnable(this);// will handle this part
        }
        _model.PlayerHand1.Text = "Your Cards";
        _model.PlayerHand1.ObjectClickedAsync += ProcessHandClickedAsync; //done.
        _model.PlayerHand1.ConsiderSelectOneAsync += OnConsiderSelectOneCardAsync; //done
        _model.PlayerHand1.BeforeAutoSelectObjectAsync += BeforeUnselectCardFromHandAsync; //done
        _model.PlayerHand1.AutoSelectedOneCompletedAsync += OnAutoSelectedHandAsync; //done.
    }
    protected override Task TryCloseAsync()
    {
        _model.Deck1.DeckClickedAsync -= Deck1_DeckClickedAsync;
        _model.Pile1.PileClickedAsync -= ProcessDiscardClickedAsync;
        _model.PlayerHand1.ObjectClickedAsync -= ProcessHandClickedAsync; //done.
        _model.PlayerHand1.ConsiderSelectOneAsync -= OnConsiderSelectOneCardAsync; //done
        _model.PlayerHand1.BeforeAutoSelectObjectAsync -= BeforeUnselectCardFromHandAsync; //done
        _model.PlayerHand1.AutoSelectedOneCompletedAsync -= OnAutoSelectedHandAsync; //done.
        return base.TryCloseAsync();
    }
    protected bool CanSendDrawMessage = true; // for games like dutch blitz, cannot send the message for drawing card.
    private async Task Deck1_DeckClickedAsync()
    {
        if (_model.PlayerHand1!.HasSelectedObject())
        {
            _toast.ShowUserErrorToast("You have to unselect all cards before drawing to prevent drawing by mistake");
            return;
        }
        if (_basicData!.MultiPlayer == true && CanSendDrawMessage == true)
        {
            await _mainGame.Network!.SendAllAsync("drawcard");
        }
        _mainGame.PlayerDraws = 0;
        _mainGame.LeftToDraw = 0;
        await _mainGame.DrawAsync();
    }
    protected abstract bool CanEnableDeck();
    protected abstract bool CanEnablePile1();
    protected virtual bool CanEnableHand()
    {
        return false; // most likely won't be this simple.
    }
    protected virtual bool AlwaysEnableHand()
    {
        return true; // most of the time, you can enable hand.  if you can't then will be here
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